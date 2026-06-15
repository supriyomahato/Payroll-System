const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5179';

export interface CreatePayrollBatchRequest {
  period: string;
  createdBy: string;
  currency: string;
  employeeLines: Array<{ employeeCode: string; grossAmount: number; deductionAmount: number }>;
}

export interface CreatePayrollBatchResponse {
  payrollBatchId: string;
  period: string;
  status: string;
  createdOnUtc: string;
  outboxEventId: string;
}

export interface PendingBatch {
  payrollBatchId: string;
  period: string;
  netAmount: number;
}

async function parseResponse<T>(response: Response): Promise<T> {
  if (response.ok) {
    return response.json();
  }

  const contentType = response.headers.get('content-type');
  if (contentType?.includes('application/json')) {
    const body = await response.json();
    const message = body.error ?? JSON.stringify(body);
    throw new Error(message);
  }

  const text = await response.text();
  throw new Error(text || 'Unknown API error');
}

export async function createPayrollBatch(request: CreatePayrollBatchRequest): Promise<CreatePayrollBatchResponse> {
  const response = await fetch(`${apiBaseUrl}/api/ledger-sync/`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  return parseResponse<CreatePayrollBatchResponse>(response);
}

export async function getPendingBatches(): Promise<PendingBatch[]> {
  const response = await fetch(`${apiBaseUrl}/api/ledger-sync/pending`);
  return parseResponse<PendingBatch[]>(response);
}

export async function syncPayrollBatch(batchId: string): Promise<boolean> {
  const response = await fetch(`${apiBaseUrl}/api/ledger-sync/${batchId}`, {
    method: 'POST',
  });

  if (response.status === 202) {
    return true;
  }

  if (response.status === 404) {
    return false;
  }

  return parseResponse<boolean>(response);
}
