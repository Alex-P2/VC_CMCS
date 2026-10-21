# Contract Monthly Claims System (CMCS)
This system has been designed for internal use at Varsity College. <br>
The CMCS allows contract lecturers to make claims for work done. Programme coordinators and academic managers can review, approve, or reject claims made by lecturers based on the information provided in the claim.
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

### Documentation
[Project Report](https://github.com/VCCT-PROG6212-2025-G1/ST10445832_PROG6212_POE/blob/main/Documentation/ST10445832_PROG6212_POE_Part1.pdf) <br>
[Figma GUI](https://github.com/VCCT-PROG6212-2025-G1/ST10445832_PROG6212_POE/tree/main/Documentation) <br>
[Project Plan](https://github.com/VCCT-PROG6212-2025-G1/ST10445832_PROG6212_POE/tree/main/Documentation/Project%20Plan)<br>
[Database UML Class Diagram](https://github.com/VCCT-PROG6212-2025-G1/ST10445832_PROG6212_POE/tree/main/Documentation/UML%20Class%20Diagram%20(Database))<br>
[YouTube Demo (Part 1)](https://youtu.be/wDHF1rNLXaI)<br>
[YouTube Demo (Part 2)](https://youtu.be/06Uq9BbWDi0)
### Declaration of AI Use
No AI tools have been used in Part 1 beyond that which has been declared in the Project Report.<br>
In Part 2 GitHub Copilot was used sparingly, with all ideas being my own. Copilot usage has been indicated in code using comments.
