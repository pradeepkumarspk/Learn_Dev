# **Adding Test cases**
Test Plans - One Test Plan per release. 
Test Suites - One Test Suite per sprint. 

**NOTE** - All test cases must be part of a Test plan and Test suite. This will endure visibility to what was tested in each release per sprint. The below process outlines adding test cases through the test plan. We selected this process  since when adding a TEST case through the work item it _seems_ like you can add a Test case to a Test Plan/Test Suite but this functionality is not currently working.

Log into Azure DevOps portal

Select RadialDropship project

Select DSvNext project

Once in the DSvNext project Navigate to Test Plans
![test plans](./testplans.png)

Once in the Test Plans, ensure you are in the most current Test plan, if not, click the back arrow and select the current Test Plan
![current](./current.PNG)

Locate the current sprint and click the 3 dots on the right, then select New Suite
![new suite](./newsuite.PNG)

Select Requirements Based Suite 
![reqsuite](./reqsuite.PNG)

In the query select ID in **Field**, = in **Operator** and the work item number in **Value** (if needed add clauses to the query to find the work item) and Click Create suites at the bottom

![query](./query.PNG)

A new Requirements based Test Suite will now be created with the name and number of the work item (please do not rename)
![newUSsuite](./newUSsuite.PNG)

Now select **Define** and start adding your test cases. Here, you have the option of searching for existing test cases, adding new test cases using the test case view (just click on the blue new Test Case box).

**Note** Test case grid view cannot be used since we have required fields inside the test cases.
![addnew](./addnew.PNG)
.
**Test Case Fields**
1. **Title** - When naming a test case include the following:  
- Functionality is being tested (Filters, Dashboard, Print etc..). 
- Role being tested - Seller or Fulfiller
- Concise description that tells the viewer without opening the test case what was being tested
    
    _Example_: Ship - Fulfiller - Ensure orders can be partially shipped for multiple sellers
    
2. **Pre Conditions**- List any preconditions that are needed to execute this test case
3. **Steps**- List **ALL** steps needed to execute this test case    
    **NOTE:** there are certain steps that have been added as "shared steps" such as, login as Fulfiller or Seller
  ![sharedsteps](./sharedsteps.PNG)
  A query window will open to search and add shared steps (one can be inserted or many)
  ![insert](./insert.PNG)
.
4. **Post Conditions** - Used for automated test cases. List any post conditions here.
5. **Related Work Links** - There should be no need to add the work item here. Since the test case was created within a requirements based  test suite and the work item was chosen, this field should be filled in with the work item by default.
6. **Required Fields** - Fill in all required fields                Regression - Yes or No
    Type - Functional, Regression, Smoke, or UI
    Test Case Priority - Critical, Important or Useful
    Method - Automated or Manual
    Risk Level - High, Low or Medium
    
Once all required fields are filled the save option will be enabled. 
If the test case is complete set the status to complete otherwise set the status to in progress
![status](./status.PNG)

  







