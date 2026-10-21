# Varsity College Contractor Monthly Claim System
##### POE SUBMISSION FOR BCAD YEAR 2 MODULE PROG6212

This system has been designed for internal use at Varsity College. <br>
The CMCS allows contract lecturers to make claims for work done. Programme coordinators and academic managers can review, approve, or reject claims made by lecturers based on the information provided in the claim. <br>
HR personnel can onboard new users, update the information of existing users, and generate invoices and statistics reports.<br>

## MVC GUI Prototype (Part 1)
#### URLs (outdated - see part 2)
/SignIn <br>
/HR <br>
/Lecturer <br>
/Lecturer/NewClaim <br>
/Lecturer/History <br>
/Lecturer/MyAccount <br>
#### Other Elements
Detail Modal <br>
Approval Confirmation Modal <br>
Rejection Confirmation Modal <br>
## Core Application Functionality (Part 2)
### URLs
/<br>
/SignIn<br>
##### Lecturers
/Lecturer<br>
/Lecturer/Claim<br>
/Lecturer/Claim/Create<br>
##### Programme Coordinators
/Approver/VerifyClaims<br>
##### Academic Managers
/Approver/ApproveClaims<br>
### Features
<ul>
  <li>Lecturer Claim Sumission</li>
  <li>Two-step (verification->approval) Oversight Process</li>
  <li>Document Encryption</li>
  <li>Real-time status updates</li>
  <li>Role-based resource access</li>
  <li>Unit Tests for claim submission, verification and approval</li>
  <li>Local SQL Server Database</li>
  <li>Sample Data Seeder</li>
  <li>Sample User Seeder</li>
</ul>

### Sample User Credentials
#### Lecturer
email: john@smith.co.za<br>
password: @Password1<br>
#### Programme Coordinator (Verifier)
email: jane@doe.co.za<br>
password: @Password1<br>
#### Academic Manager (Approver)
email: sam@iam.co.za<br>
password: @Password1<br>
<br>
Users can also be found in Data/IdentitySeeder.cs<br>

### Additional Notes
<ul>
  <li>Claim Statuses: Pending (Orange), Verified (Blue), Approved (Green), Rejected (Black)</li>
  <li>Sources Referenced during development are listed at the top of the relevant code file</li>
  <li>Permitted file types: .pdf .docx .xlsx</li>
  <li>Maximum permitted file size: 5MB</li>
  <li>If a valid SQL Server connection string is provided, the application should automatically create the database on startup (Default: Data Source=(localdb)\\MSSQLLocalDB;Database=ST10445832_PROG6212_POE;Trusted_Connection=True;)</li>
  <li>No actionable lecturer feedback was provided after part 1, and thus could not be addressed in part 2</li>
</ul>

<h2>Automation and HR - (Part 3)</h2>

<h3>Lecturer Feedback on Part 2</h3>
<p>Application crashes when trying to submit a claim with a large file (200MB). Ensure that the application does not crash when uploading a large file (200MB), and that an appropriate error message is displayed to the user.</p>
<p>To address this bug, I disabled model binding for documents during claim submission, increased the maximum request size, and read the file data from the request using HttpContext.Request.Features.GetRequiredFeature.</p>

<h3>Part 3 Features</h3>
<ul>
  <li>Auto-population of Claimant name and hourly rate during claim submission</li>
  <li>Automated claim value calculation</li>
  <li>Automated claim approval with manual fallback</li>
  <li>Invoice generation (single or multi-claim)</li>
  <li>User and performance metrics reporting</li>
  <li>Session idle timeout</li>
</ul>

<h3>Added HR User Role & Area</h3>
<p>HR users can onboard other users, update user information, process claims, generate invoices, and generate reports</p>


<h3>Sample Users</h3>
<h4>Lecturer</h4>
<p>Email: john@smith.co.za</p>
<p>Password: @Password1</p>

<h4>Programme Coordinator</h4>
<p>Email: jane@doe.co.za</p>
<p>Password: @Password1</p>

<h4>Academic Manager</h4>
<p>Email: sam@iam.co.za</p>
<p>Password: @Password1</p>

<h4>HR</h4>
<p>Email: alex@gmail.com</p>
<p>Password: @Password1</p>

<h3>NuGet Packages</h3>
<ul>
  <li>Miscrosoft.AspNetCore.Authentication.Cookies</li>
  <li>Microsoft.AspNetCore.Identity.EntityFrameworkCore</li>
  <li>Microsoft.AspNetCore.Identity.UI</li>
  <li>Microsoft.EntityFrameworkCore</li>
  <li>Microsoft.EntityFrameworkCore.SqlServer</li>
  <li>Microsoft.NET.Test.Sdk</li>
  <li>Moq</li>
  <li>MSTest</li>
  <li>QuestPDF</li>
  <li>ScottPlot</li>
</ul>

<h3>Running the Application</h3>
<ol>
  <li>Clone this repository/download as zip</li>
  <li>Open ST10445832_PROG6212_POE.sln in VisualStudio</li>
  <li>If the application doesn't run the first try, run the commands 'add-migration >migrationName<' and 'update-database' in the package manager console</li>
  <li>Run the application</li>
</ol>

### Documentation
[Project Report](https://github.com/Alex-P2/VC_CMCS/blob/main/Documentation/ST10445832_PROG6212_POE_Part1.pdf) <br>
[Figma GUI](https://github.com/Alex-P2/VC_CMCS/blob/main/Documentation/Figma%20GUI%20Design/ST10445832%20Contract%20Monthly%20Claim%20System%20Figma.pdf) <br>
[Project Plan](https://github.com/Alex-P2/VC_CMCS/blob/main/Documentation/Project%20Plan/ST10445832%20Project%20Plan%20-%20Gantt.pdf)<br>
[Database UML Class Diagram](https://github.com/Alex-P2/VC_CMCS/blob/main/Documentation/UML%20Class%20Diagram%20(Database)/ST10445832_PROG6212_POEv3.png)<br>
[Presentation](https://github.com/Alex-P2/VC_CMCS/blob/main/Documentation/Powerpoint%20Presentation%20(Part%203)/ST01445832%20PROG6212%20CMCS%20Presentation.pdf)<br>
[YouTube Demo (Part 1)](https://youtu.be/wDHF1rNLXaI)<br>
[YouTube Demo (Part 2)](https://youtu.be/06Uq9BbWDi0)<br>
[YouTube Demo (Part 3)](https://www.youtube.com/watch?v=hT48b6yZzzk)<br>

<h3>References</h3>
https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API/Using_Fetch
https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/file
https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/optgroup
https://geeksarray.com/blog/how-to-use-areas-in-asp-net-core-mvc
https://getbootstrap.com/docs/5.3/components/modal/
https://getbootstrap.com/docs/5.3/components/toasts/
https://getbootstrap.com/docs/5.3/content/tables/
https://icons.getbootstrap.com/icons/check/
https://icons.getbootstrap.com/icons/person-circle/
https://icons.getbootstrap.com/icons/x/
https://improveandrepeat.com/2019/10/working-with-temporary-files-in-net/
https://jakeydocs.readthedocs.io/en/latest/fundamentals/error-handling.html
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-9.0
https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-9.0#upload-large-files-with-streaming
https://learn.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-9.0
https://learn.microsoft.com/en-us/aspnet/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity
https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.frombodyattribute?view=aspnetcore-9.0
https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.join?view=net-8.0
https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations
https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete
https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database
https://learnentityframeworkcore.com/relationships
https://medium.com/@roshanj100/users-and-roles-seeding-in-asp-net-core-identity-with-entity-framework-core-a-step-by-step-guide-28e6f76a18db
https://putridparrot.com/blog/storing-binary-data-using-entity-framework/
https://questpdf.com/api-reference/page/basics.html
https://questpdf.com/api-reference/table/basics.html
https://questpdf.com/getting-started.html
https://w3docs.com/snippets/css/how-to-customize-file-inputs.html
https://www.csharp411.com/read-file-into-byte-array/
https://www.csharptutorial.net/csharp-linq/linq-inner-join/
https://www.learnentityframeworkcore.com/relationships

### Declaration of AI Use
No AI tools have been used in Part 1 beyond that which has been declared in the Project Report.<br>
In Part 2 GitHub Copilot was used sparingly, with all ideas being my own. Copilot usage has been indicated in code using comments.<br>
As in Part 2 GitHub Copilot was used in part 3, indicated in comments in code.<br>
Additionally, I used Grok to sort and organise my references for this README file. Chat link here: [View Grok Chat](https://grok.com/share/c2hhcmQtNA_d8a8db3b-0f18-4b42-93a0-f27611f1271a)
