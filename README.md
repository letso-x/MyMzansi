# MyMzansi Municipal Services

## Application

MyMzansi is a Windows Forms municipal services application for reporting service issues. It provides a main menu and an issue-reporting workflow suitable for municipal PoE demonstration.

## Requirements

- Visual Studio with Windows Forms support.
- .NET Framework 4.8 Developer Pack.
- No third-party dependencies are required.

## How to compile and run

1. Open `MyMzansi.slnx` in Visual Studio.
2. Restore packages if Visual Studio requests it. This project has no third-party package dependencies.
3. Build the solution using **Build > Build Solution**.
4. Run the application using **Debug > Start Without Debugging** or the Start button.

## How to use

1. Start the application.
2. Select **Report Issues** from the main menu.
3. Enter the issue subject and location.
4. Select an issue category.
5. Enter a description of the issue.
6. Optionally attach an image or document using **Attach Image / Document**.
7. Observe the progress bar and encouraging status messages as the form is completed.
8. Select **Submit** to validate and save the report.
9. Select **Back to Main Menu** on the issue dashboard when finished.

## Features

- Main menu with Report Issues, Local Events and Announcements, and Service Request Status options.
- Report Issues workflow with subject, location, category, and description fields.
- Validation with clear user feedback for missing required information.
- Optional JPG, JPEG, PNG, GIF, BMP, PDF, DOC, and DOCX attachments.
- Issue dashboard showing submitted issue summaries and details.
- In-memory issue storage using a strongly typed `List<Issue>`.
- Dynamic progress indicator and encouraging engagement messages.
- Success messages and form reset after valid submission.
- Navigation between the main menu, dashboard, and report form.

## Data handling

Reported issues are stored in the application's in-memory `List<Issue>` data structure. The application does not use a database, so reports are not persisted after the application closes.
