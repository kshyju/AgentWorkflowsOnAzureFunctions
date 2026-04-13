# Human-in-the-Loop Sample

This sample demonstrates the human-in-the-loop pattern — a workflow that pauses at approval gates and waits for external input before continuing. It uses `RequestPort` to model pause points.

## Workflow: ExpenseReimbursement

| Step | Type | Description |
|------|------|-------------|
| **CreateApprovalRequest** | Executor | Builds an approval request from the expense ID |
| **ManagerApproval** | RequestPort | Pauses for manager approval |
| **PrepareFinanceReview** | Executor | Prepares the request for parallel finance review |
| **BudgetApproval** | RequestPort | Pauses for budget team approval (parallel) |
| **ComplianceApproval** | RequestPort | Pauses for compliance team approval (parallel) |
| **ExpenseReimburse** | Executor | Processes the reimbursement after all approvals |

```
CreateApprovalRequest -> ManagerApproval -> PrepareFinanceReview -> [BudgetApproval, ComplianceApproval] (parallel) -> ExpenseReimburse
```

The framework auto-generates three HTTP endpoints:

- `POST /api/workflows/ExpenseReimbursement/run` — Start the workflow
- `GET /api/workflows/ExpenseReimbursement/status/{runId}` — Check status and pending approvals
- `POST /api/workflows/ExpenseReimbursement/respond/{runId}` — Submit an approval response

## Environment Setup

See the [README.md](../README.MD) file in the parent directory for complete setup instructions, including:

- Prerequisites installation
- Durable Task Scheduler setup
- Storage emulator configuration

## Running the Sample

1. **Start the Function App**:

   ```bash
   cd HumanInTheLoop
   func start
   ```

2. **Start the workflow**:

   ```bash
   curl -X POST http://localhost:7071/api/workflows/ExpenseReimbursement/run \
     -H "Content-Type: text/plain" \
     -d "EXP-2025-042"
   ```

3. **Check status** (note the `runId` from the start response):

   ```bash
   curl http://localhost:7071/api/workflows/ExpenseReimbursement/status/{runId}
   ```

4. **Submit approvals** one at a time. First, the manager:

   ```bash
   curl -X POST http://localhost:7071/api/workflows/ExpenseReimbursement/respond/{runId} \
     -H "Content-Type: application/json" \
     -d '{"eventName": "ManagerApproval", "response": {"approved": true, "comments": "Looks good"}}'
   ```

   Then budget and compliance (these can be submitted in any order):

   ```bash
   curl -X POST http://localhost:7071/api/workflows/ExpenseReimbursement/respond/{runId} \
     -H "Content-Type: application/json" \
     -d '{"eventName": "BudgetApproval", "response": {"approved": true, "comments": "Within budget"}}'
   ```

   ```bash
   curl -X POST http://localhost:7071/api/workflows/ExpenseReimbursement/respond/{runId} \
     -H "Content-Type: application/json" \
     -d '{"eventName": "ComplianceApproval", "response": {"approved": true, "comments": "Compliant"}}'
   ```
