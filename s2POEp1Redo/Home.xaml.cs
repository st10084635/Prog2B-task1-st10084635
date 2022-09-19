using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CourseCalculations;//refrencing the DLL

namespace s2POEp1Redo
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        //public objects of classes that is in the DLL and a public list
        Module mod = new Module();
        List<Module> module = new List<Module>();
        Semester sem = new Semester();
        Calc calc = new Calc();

        public Home()
        {
            InitializeComponent();
        }

        //method that inserts values into the list
        public void AddModuleToList(string code, string name, int credits, int classHrsPerWeek, int numWeeks, int selfStudyHrsPerWeek)
        {
            module.Add(new Module(code, name, credits, classHrsPerWeek, numWeeks, calc.CalcHrsPerWeek(credits, classHrsPerWeek, numWeeks)));
        }

        

        //method that displays the error message box 
        public void Error()
        {
            MessageBox.Show("Please insert the correct value types in the textboxes.", "Warning", MessageBoxButton.OK);
        }

        //Method that clears the all the textboxes that is related to inserting a module in the program
        public void ClearCourseTB()
        {
            CourseNameTB.Text = null;
            CourseCodeTB.Text = null;
            CreditsTB.Text = null;
            classHrsTB.Text = null;
            WeekSemesterTB.Text = null;

            
        }

        //method that clears all the text boxes that is related to adding hours to the worked that has been done by the user
        public void ClearHrsTable()
        {
            DateOfStudyTB.Text = null;
            CodeTB.Text = null;
            HrsWorkedTB.Text = null;
        }

        //button click method 
        private void addCourseBT_Click(object sender, RoutedEventArgs e)
        {
            //an if statement that displays a message box first to confirm if the values that are in the textboxes are correct
            if (MessageBox.Show("Are you sure that the entry is correct?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //try catch for error handling
                try
                {
                    //creating new variables as temporary storage for the values entered
                    string name = CourseNameTB.Text;
                    string code = CourseCodeTB.Text;
                    int credits = Convert.ToInt32(CreditsTB.Text);
                    int classHrsPerWeek = Convert.ToInt32(classHrsTB.Text);
                    int numWeeks = Convert.ToInt32(WeekSemesterTB.Text);
                    int selfStudyHrsPerWeek = calc.CalcHrsPerWeek(credits, classHrsPerWeek, numWeeks);//storing the value of that is calclated by the method




                    //using the object of the modules class to store the temporary variables values into the variables that are in the class
                    mod.name = name;
                    mod.code = code;
                    mod.credits = credits;
                    mod.classHrsPerWeek = classHrsPerWeek;
                    mod.numWeeks = numWeeks;
                    mod.selfStudyHrsPerWeek = selfStudyHrsPerWeek;

                    //inserting values into a list
                    AddModuleToList(code, name, credits, classHrsPerWeek, numWeeks, calc.CalcHrsPerWeek(credits, classHrsPerWeek, numWeeks));

                    //adding the values to the data grid view
                    //CourseDetailsDG.Items.Add(mod);

                    addModule();
                    
                    //calling method that clears text boxes
                    ClearCourseTB();
                }
                catch(Exception)
                {
                    //calling method that displays the error message
                    Error();
                }
            }

        }
        
        //button click event that handles the hrs that the user worked on a module
        private void AddHrsBT_Click(object sender, RoutedEventArgs e)
        {
            //try catch for error handling
            try
            {
                //an if statement that displays a message box first to confirm if the values that are in the textboxes are correct
                if (MessageBox.Show("Are you sure that the entry is correct?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //new temporary vairibles that stores the value of the text boxes
                    DateTime startDate = DateTime.Parse(DateOfStudyTB.Text);
                    string code = CodeTB.Text;
                    int hrsSpentThisWeek = Convert.ToInt32(HrsWorkedTB.Text);

                    //var result = Convert.ToString(module.Where(m => m.code.Contains(code)));


                    //an if statement that will only insert the values if the course code that was inserted into the temp variable is equal to the code that waspreviously inserted into module code
                    if (code == mod.code /*result*/ && sem.selfStudyHrsPerWeek == 0)
                    {
                        //using the object of the semester class to store the temporary variables values into the variables that are in the class
                        sem.startDate = startDate;
                        sem.code = code;
                        sem.hrsSpentThisWeek = hrsSpentThisWeek;

                        //sets the self study hrs of the hrs worked to the module self study hours
                        sem.selfStudyHrsPerWeek = mod.selfStudyHrsPerWeek;

                        //variable that calculates the remaining Hrs left 
                        sem.remainingHrs = sem.selfStudyHrsPerWeek - hrsSpentThisWeek;

                        //adds the values to the datagrid
                        HrsDG.Items.Add(sem);

                        //clears the text boxes of the hrs worked
                        ClearHrsTable();
                    }
                    else if (code == mod.code && sem.selfStudyHrsPerWeek != 0)
                    {
                        //storing all the values of the temporary variables into the semester class variables
                        sem.startDate = startDate;
                        sem.code = code;
                        sem.hrsSpentThisWeek = hrsSpentThisWeek;

                        //setting the sem selfstudy hours equal to the mod self study hours
                        sem.selfStudyHrsPerWeek = mod.selfStudyHrsPerWeek;

                        //setting the remaining hrs equal to itself minus hrs spent this week
                        sem.remainingHrs = sem.remainingHrs - hrsSpentThisWeek;

                        //adds the values to the datagrid
                        HrsDG.Items.Add(sem);

                        //clears the text boxes of the hrs worked
                        ClearHrsTable();
                    }

                    //else statement to display error message if the code that was typed in does not match
                    else
                    {
                        //message box that displays error message
                        MessageBox.Show("Current module does not exist please insert a value that was added previously!" +
                            "\nOr double check the spelling of your module code so that it is similiar to a module code that was added!", "Warning", MessageBoxButton.OK);
                    }
                }
            }
            //catch for error handeling
            catch (Exception)
            {
                //calling method that displays error message
                Error();
            }
            
                
        }

        //method that adds the value saved in the list into the data grid using linq
        public void addModule()
        {
            //linq that gets all the values to insert it into 
            var dgPop = from m in module
                        select (m.name, m.code, m.credits, m.classHrsPerWeek, m.numWeeks, calc.CalcHrsPerWeek(m.credits, m.classHrsPerWeek, m.numWeeks));

            //for each loop that inserts values into the data grid view
            foreach (var element in module)
            {
                //removes the first row to stop duplications
                CourseDetailsDG.Items.Remove(element);
                //inserting the next row
                CourseDetailsDG.Items.Add(element);
            }
        }
        
        //button click event that will close the program
        private void EndBT_Click(object sender, RoutedEventArgs e)
        {
            //closing the window
            this.Close();
        }
    }
}
