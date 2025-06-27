namespace ToDoList
{
    public class Test_semgrep
    {
        public int SomeRiskyOperation()
        {
            return 0;
        }
        public void errors()
        {
            string apiKey = "test_1574849332674554558";

            string userName = "admin";

            var query = "SELECT * FROM Users WHERE Name = '" + userName + "'";


            try
            {
                SomeRiskyOperation();
            }
            catch (Exception)  {}

        }


    }
}
