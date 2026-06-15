import { useEffect, useMemo, useState } from 'react';
import { createPayrollBatch, getPendingBatches, syncPayrollBatch } from './api';
import './App.css';

interface EmployeeLine {
  employeeCode: string;
  grossAmount: number;
  deductionAmount: number;
}

interface PendingBatch {
  payrollBatchId: string;
  period: string;
  netAmount: number;
}

function emptyLine(): EmployeeLine {
  return { employeeCode: '', grossAmount: 0, deductionAmount: 0 };
}

function App() {
  const [period, setPeriod] = useState('2026-06');
  const [createdBy, setCreatedBy] = useState('system');
  const [currency, setCurrency] = useState('USD');
  const [employeeLines, setEmployeeLines] = useState<EmployeeLine[]>([emptyLine()]);
  const [pendingBatches, setPendingBatches] = useState<PendingBatch[]>([]);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const netAmount = useMemo(
    () => employeeLines.reduce((sum, line) => sum + (line.grossAmount - line.deductionAmount), 0),
    [employeeLines],
  );

  useEffect(() => {
    void refreshPending();
  }, []);

  async function refreshPending() {
    setLoading(true);
    setError(null);

    try {
      const batches = await getPendingBatches();
      setPendingBatches(batches);
    } catch (err) {
      setError((err as Error).message || 'Unable to load pending batches.');
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const response = await createPayrollBatch({
        period,
        createdBy,
        currency,
        employeeLines: employeeLines.map(line => ({
          employeeCode: line.employeeCode,
          grossAmount: line.grossAmount,
          deductionAmount: line.deductionAmount,
        })),
      });

      setMessage(`Created batch ${response.payrollBatchId} for ${response.period}.`);
      setEmployeeLines([emptyLine()]);
      await refreshPending();
    } catch (err) {
      setError((err as Error).message || 'Unable to create batch.');
    } finally {
      setLoading(false);
    }
  }

  async function handleSync(batchId: string) {
    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const synced = await syncPayrollBatch(batchId);
      if (synced) {
        setMessage(`Payroll batch ${batchId} synced successfully.`);
        await refreshPending();
      } else {
        setError('Batch not found or already synced.');
      }
    } catch (err) {
      setError((err as Error).message || 'Unable to sync payroll batch.');
    } finally {
      setLoading(false);
    }
  }

  function updateLine(index: number, changes: Partial<EmployeeLine>) {
    setEmployeeLines(current => current.map((line, idx) => (idx === index ? { ...line, ...changes } : line)));
  }

  function addLine() {
    setEmployeeLines(current => [...current, emptyLine()]);
  }

  function removeLine(index: number) {
    setEmployeeLines(current => current.filter((_, idx) => idx !== index));
  }

  return (
    <div className="app-shell">
      <header className="hero">
        <h1>Payroll Ledger Sync</h1>
        <p>Manage payroll batches, review pending syncs, and trigger ledger sync operations.</p>
      </header>

      <section className="panel">
        <h2>Create payroll batch</h2>
        <form onSubmit={handleSubmit}>
          <div className="field-grid">
            <label>
              Period
              <input type="month" value={period} onChange={e => setPeriod(e.target.value)} required />
            </label>
            <label>
              Created by
              <input value={createdBy} onChange={e => setCreatedBy(e.target.value)} required />
            </label>
            <label>
              Currency
              <input value={currency} onChange={e => setCurrency(e.target.value)} required />
            </label>
          </div>

          <div className="lines-section">
            <div className="lines-header">
              <h3>Employee payroll lines</h3>
              <button type="button" onClick={addLine}>Add line</button>
            </div>

            {employeeLines.map((line, index) => (
              <div key={index} className="line-row">
                <label>
                  Employee code
                  <input
                    value={line.employeeCode}
                    onChange={e => updateLine(index, { employeeCode: e.target.value })}
                    required
                  />
                </label>
                <label>
                  Gross amount
                  <input
                    type="number"
                    min="0"
                    step="0.01"
                    value={line.grossAmount}
                    onChange={e => updateLine(index, { grossAmount: Number(e.target.value) })}
                    required
                  />
                </label>
                <label>
                  Deductions
                  <input
                    type="number"
                    min="0"
                    step="0.01"
                    value={line.deductionAmount}
                    onChange={e => updateLine(index, { deductionAmount: Number(e.target.value) })}
                    required
                  />
                </label>
                <button type="button" className="remove-button" onClick={() => removeLine(index)}>
                  Remove
                </button>
              </div>
            ))}
          </div>

          <div className="summary-row">
            <div>Net amount: {netAmount.toFixed(2)} {currency}</div>
            <button type="submit" disabled={loading}>Create batch</button>
          </div>
        </form>
      </section>

      <section className="panel">
        <div className="panel-header">
          <h2>Pending payroll batches</h2>
          <button type="button" onClick={refreshPending} disabled={loading}>Refresh</button>
        </div>

        {loading && <div className="status-message">Loading…</div>}
        {message && <div className="status-message success">{message}</div>}
        {error && <div className="status-message error">{error}</div>}

        <table>
          <thead>
            <tr>
              <th>Batch ID</th>
              <th>Period</th>
              <th>Net amount</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {pendingBatches.length === 0 ? (
              <tr>
                <td colSpan={4}>No pending batches found.</td>
              </tr>
            ) : (
              pendingBatches.map(batch => (
                <tr key={batch.payrollBatchId}>
                  <td>{batch.payrollBatchId}</td>
                  <td>{batch.period}</td>
                  <td>{batch.netAmount.toFixed(2)}</td>
                  <td>
                    <button type="button" onClick={() => void handleSync(batch.payrollBatchId)} disabled={loading}>
                      Sync
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </section>
    </div>
  );
}

export default App;
