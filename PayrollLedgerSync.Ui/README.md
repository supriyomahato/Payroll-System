# Payroll Ledger Sync UI

A React + Vite frontend for the Payroll Ledger Sync API.

## Run locally

1. Install dependencies:

   ```bash
   cd "PayrollLedgerSync.Ui"
   npm install
   ```

2. Start development server:

   ```bash
   npm run dev
   ```

3. Open the app in the browser at the URL shown by Vite.

## API configuration

The UI calls the backend at `VITE_API_BASE_URL` if provided. Otherwise it uses `http://localhost:5000`.

Create a `.env` file in `PayrollLedgerSync.Ui` with:

```env
VITE_API_BASE_URL=http://localhost:5000
```

## Features

- Create payroll batches with employee payroll lines
- View pending batches awaiting sync
- Trigger ledger sync for a specific batch
