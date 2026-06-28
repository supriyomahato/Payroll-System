# Payroll Ledger Sync System - User Guide

**Version:** 1.0  
**Last Updated:** June 2026

---

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [System Features](#system-features)
4. [Creating a Payroll Batch](#creating-a-payroll-batch)
5. [Managing Batches](#managing-batches)
6. [Synchronizing to General Ledger](#synchronizing-to-general-ledger)
7. [Viewing Batch Details](#viewing-batch-details)
8. [Troubleshooting](#troubleshooting)
9. [FAQ](#faq)

---

## Introduction

### What is the Payroll Ledger Sync System?

The **Payroll Ledger Sync System** is an enterprise-grade application designed to streamline payroll processing and synchronize payroll data with your company's general ledger. It automates the creation of payroll batches, validates accounting entries, and ensures all financial data is accurately recorded.

### Key Benefits

- **Automated Batch Processing** - Create and manage payroll batches efficiently
- **Real-time Synchronization** - Sync payroll data to your general ledger instantly
- **Data Validation** - Automatic verification that all accounting entries balance
- **User-Friendly Interface** - Intuitive web-based interface for easy navigation
- **Reliable Processing** - Enterprise-grade reliability with message queuing
- **Audit Trail** - Complete tracking of all batch operations

### Who Should Use This System?

- Payroll Managers
- Accounting Managers
- Finance Administrators
- Anyone responsible for payroll processing and general ledger management

---

## Getting Started

### System Requirements

- Modern web browser (Chrome, Firefox, Safari, Edge)
- Internet connection
- Active user account with appropriate permissions

### Accessing the Application

1. Open your web browser
2. Navigate to the Payroll Ledger Sync System URL provided by your administrator
3. You will see the application dashboard with available options

### Dashboard Overview

The dashboard displays:
- **Create New Batch** - Start a new payroll batch
- **Pending Batches** - View batches awaiting synchronization
- **Recent Activity** - Summary of recent payroll operations

---

## System Features

### 1. Payroll Batch Creation

Create organized payroll batches for specific periods with employee compensation details.

### 2. Ledger Entry Management

Automatically generate and manage accounting entries that correspond to payroll information.

### 3. Balance Validation

The system automatically validates that all accounting entries balance (total debits = total credits).

### 4. Batch Synchronization

Send completed and validated payroll batches to your general ledger system.

### 5. Status Tracking

Monitor the status of your payroll batches throughout the entire process.

### 6. Comprehensive Reporting

View detailed information about all processed batches and their synchronization status.

---

## Creating a Payroll Batch

### Step 1: Access the Batch Creation Form

1. Click on **"Create New Batch"** button on the dashboard
2. The Batch Creation form will open

### Step 2: Enter Batch Information

Fill in the following required fields:

#### **Payroll Period**
- **Format:** YYYY-MM (e.g., 2024-06 for June 2024)
- **Description:** The month and year for which this payroll batch applies
- **Requirement:** Must be in the specified format

#### **Currency**
- **Default:** USD
- **Options:** Select from available currency codes
- **Description:** The currency in which all amounts in this batch are denominated

#### **Created By**
- **Input:** Your name or identifier
- **Description:** Records who created the batch for audit purposes

### Step 3: Add Employee Payroll Lines

For each employee receiving payroll in this batch:

1. Click **"Add Employee"** button
2. Enter the following information:

#### Employee Details

| Field | Description | Example | Notes |
|-------|-------------|---------|-------|
| **Employee Code** | Unique identifier for the employee | EMP001 | Required |
| **Gross Amount** | Total compensation before deductions | 5000.00 | Must be positive number |
| **Deductions** | Total deductions from gross pay | 1000.00 | Must not exceed gross amount |
| **Net Pay** | *Auto-calculated* | 4000.00 | Automatically calculated (Gross - Deductions) |

### Step 4: Add Accounting Entries

The system automatically creates ledger entries for each employee payroll. You can view or modify these entries:

1. For each employee, accounting entries are created for:
   - **Salary Expense Account** - Debit entry with gross amount
   - **Employee Deductions Account** - Credit entry for deductions
   - **Cash/Bank Account** - Credit entry for net pay

#### Entry Details

| Field | Description |
|-------|-------------|
| **Account Code** | The general ledger account number |
| **Debit Amount** | Amount debited to the account (if applicable) |
| **Credit Amount** | Amount credited to the account (if applicable) |
| **Description** | Description of the transaction |

### Step 5: Review Balance

The system automatically displays:
- **Total Debits** - Sum of all debit entries
- **Total Credits** - Sum of all credit entries
- **Balance Status** - ✓ Balanced or ✗ Not Balanced

**Important:** Your batch must be balanced (Total Debits = Total Credits) before submission.

### Step 6: Submit the Batch

1. Review all information for accuracy
2. Click **"Submit Batch"** button
3. The system will process and save your batch
4. You will receive a confirmation with:
   - Batch ID (for reference)
   - Submission timestamp
   - Current status: "Submitted"

---

## Managing Batches

### Viewing All Batches

1. Click on **"All Batches"** or **"Pending Batches"** from the menu
2. You will see a list of all payroll batches with:
   - Batch ID
   - Payroll Period
   - Total Net Amount
   - Current Status
   - Date Created
   - Created By

### Batch Statuses

| Status | Description | Next Action |
|--------|-------------|------------|
| **Draft** | Batch is being prepared but not yet submitted | Continue editing or submit |
| **Submitted** | Batch has been submitted and is awaiting sync | Review and sync to ledger |
| **Synced** | Batch has been successfully synchronized to the ledger | Monitor in ledger system |
| **Error** | An error occurred during processing | Contact support or retry |

### Filtering and Searching

1. Use the **search bar** to find batches by:
   - Batch ID
   - Payroll Period
   - Created By name

2. Use **filters** to view batches by:
   - Status (Draft, Submitted, Synced)
   - Date Range
   - Currency

---

## Synchronizing to General Ledger

### What is Synchronization?

Synchronization sends your validated payroll batch to your general ledger system, where it becomes official accounting records.

### Prerequisites for Synchronization

Before syncing, ensure:
- ✓ Batch status is "Submitted"
- ✓ All accounting entries are balanced
- ✓ All required information is complete
- ✓ General ledger system is accessible

### Synchronizing a Batch

#### Method 1: From Batch List

1. Find the batch you want to sync in the pending batches list
2. Click on the batch to open its details
3. Click **"Sync to Ledger"** button
4. Confirm the action in the confirmation dialog
5. The system will process the synchronization
6. Status will change to "Synced" upon completion

#### Method 2: Bulk Synchronization

1. Go to **"Pending Batches"** page
2. Select multiple batches using checkboxes
3. Click **"Sync Selected"** button at the top
4. Confirm the action
5. System will process all selected batches
6. You'll receive a summary of results

### Monitoring Synchronization

After clicking sync:

1. **Processing Status** - You'll see a "Processing..." indicator
2. **Real-time Updates** - Status updates automatically
3. **Completion Notification** - You'll be notified when sync completes
4. **Error Messages** - Any errors will be displayed with instructions

### Synchronization Success

When synchronization completes successfully:
- Batch status changes to **"Synced"**
- Timestamp shows sync completion time
- Batch is now recorded in the general ledger
- You cannot modify the batch

---

## Viewing Batch Details

### Opening Batch Details

1. Click on any batch in the list
2. The detailed view opens showing:

### Information Displayed

#### **Batch Header**
- Batch ID
- Status and status color indicator
- Payroll Period
- Currency
- Created By
- Creation Date and Time

#### **Employee Payroll Section**
- Table showing all employees in the batch
- Each row contains:
  - Employee Code
  - Gross Amount
  - Deductions
  - Net Pay
  - Edit/Delete options

#### **Accounting Entries Section**
- Complete list of ledger entries
- Shows:
  - Account Code
  - Account Description
  - Debit Amount (if applicable)
  - Credit Amount (if applicable)
  - Entry Description

#### **Balance Summary**
- Total Gross Pay
- Total Deductions
- Total Net Pay
- Total Debits
- Total Credits
- Balance Status

### Actions Available

**From Detail View**, depending on batch status:

| Action | Available For | Description |
|--------|--------------|-------------|
| **Edit** | Draft batches | Modify batch details |
| **Add Employee** | Draft/Submitted | Add more employees |
| **Delete Employee** | Draft/Submitted | Remove employee line |
| **Sync to Ledger** | Submitted | Synchronize to general ledger |
| **Download Report** | All | Export batch details as PDF |
| **Print** | All | Print batch details |

---

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: Batch Will Not Balance

**Problem:** Error message "Accounting entries do not balance"

**Solution:**
1. Review all debit entries (should equal gross salaries)
2. Review all credit entries (should equal deductions + net pay)
3. Check that employee deductions don't exceed gross pay
4. Ensure all amounts are entered correctly
5. Add missing accounting entries if needed

#### Issue 2: Cannot Submit Batch

**Problem:** Submit button is disabled or shows error

**Possible Causes:**
- Required fields are not filled (check all fields have values)
- Batch is not balanced (total debits ≠ total credits)
- Employee deductions exceed gross amount
- Invalid data format in any field

**Solution:**
1. Check all required fields are completed
2. Review the error message for specifics
3. Correct the identified issues
4. Try submitting again

#### Issue 3: Synchronization Failed

**Problem:** Sync to Ledger results in error

**Possible Causes:**
- General ledger system is temporarily unavailable
- Network connectivity issue
- Batch data format incompatibility
- General ledger is at capacity

**Solution:**
1. Check your internet connection
2. Verify the general ledger system is online
3. Wait a few minutes and retry
4. If problem persists, contact technical support

#### Issue 4: Cannot Find a Batch

**Problem:** A batch I created is not appearing in the list

**Solutions:**
1. Use the search function with the batch ID
2. Check different status filters (it might be in Draft)
3. Verify the date range filter includes the batch creation date
4. Clear all filters and search again
5. Refresh the page (Ctrl+R or Cmd+R)

#### Issue 5: Data Lost After Closing Browser

**Problem:** Unsaved changes were lost

**Prevention:**
1. Always click "Submit" to save batches
2. Unsaved Draft batches may be lost if browser is closed
3. Use "Save Progress" for draft batches (if available)

---

## FAQ

### General Questions

**Q: What if I make a mistake in a submitted batch?**

A: Once a batch is "Synced", it cannot be modified as it's been recorded in the general ledger. Contact your accounting manager or administrator for correction procedures. For "Submitted" (not yet synced) batches, you can typically edit certain fields.

**Q: Can I submit partial batches?**

A: Yes, you can create and submit batches for any subset of employees. However, all batches must balance before submission.

**Q: How long does synchronization take?**

A: Typically, synchronization completes within seconds to minutes depending on system load. You'll see a status indicator showing progress.

**Q: Can I undo a synchronization?**

A: No, once synchronized, batches are committed to the general ledger. Work with your administrator if reversal is needed.

---

### Technical Questions

**Q: What browsers are supported?**

A: All modern browsers are supported:
- Chrome (latest version)
- Firefox (latest version)
- Safari (latest version)
- Edge (latest version)

**Q: Is my data secure?**

A: Yes, all data is encrypted in transit and at rest. Access is restricted to authorized users only.

**Q: Can I access this system from my mobile device?**

A: The system is optimized for desktop browsers. Mobile access may be limited but should be readable on tablets.

**Q: How is my data backed up?**

A: All payroll data is automatically backed up according to company policy. Contact your IT administrator for specific details.

---

### Process Questions

**Q: What are the required fields when creating a batch?**

A: 
- Payroll Period (YYYY-MM format)
- Currency
- Created By
- At least one employee with gross amount and deductions
- Balanced accounting entries

**Q: Can I edit a batch after submission?**

A: No, submitted batches cannot be edited. To make changes, you would need to create a new batch or contact an administrator.

**Q: What happens if total debits don't equal total credits?**

A: The system will not allow submission. You must add/modify accounting entries until the batch balances.

**Q: Can multiple people work on the same batch?**

A: No, batches are created by individual users. To collaborate, create the batch and share the details before submission.

---

### Support and Help

**Q: Where can I get additional help?**

A: 
- Contact your IT Help Desk
- Email: payroll-support@company.com
- Phone: (555) 123-4567
- Check the system help documentation
- Ask your manager or department administrator

**Q: How do I report a bug or issue?**

A: 
1. Note the specific steps that caused the issue
2. Include any error messages displayed
3. Send to: payroll-support@company.com
4. Contact your system administrator

**Q: How do I request a new feature?**

A: Submit feature requests to your manager or directly to the development team through the proper channels.

---

## Best Practices

### For Optimal System Performance

1. **Review Before Submitting** - Always review batch details before submission
2. **Use Correct Period Format** - Always use YYYY-MM format for payroll periods
3. **Save Regularly** - Submit batches promptly to avoid losing work
4. **Document Creators** - Clearly identify batch creator in the "Created By" field
5. **Monitor Synchronization** - Check batch status after syncing
6. **Keep Records** - Note batch IDs for audit trail purposes
7. **Regular Backups** - Periodically export batch reports for records
8. **Stay Updated** - Check for system updates and announcements

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | June 2026 | Initial release |

---

## Document Information

**Author:** Payroll System Team  
**Last Updated:** June 28, 2026  
**Next Review:** December 2026

For the most up-to-date information, refer to the system help documentation within the application.

---

*© 2026 Your Company. All rights reserved.*
