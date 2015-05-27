namespace System.Data.Access
{
    class JoinTable
    {
        public object obj1;
        //static Dictionary<object, string> joinRefCounter = new Dictionary<object, string>();
        string tinf = "";

        public JoinTable(object obj1, ref int count)
        {
            if (obj1.GetType() != typeof(Join))
            {
                tinf = "t" + count.ToString();
                count++;
                //joinRefCounter.Add(this, tinf);

            }// TODO: Complete member initialization
            this.obj1 = obj1;
        }

        public string Alias { get { return tinf; } }

        public string getField(string name)
        {
            if (obj1.GetType() != typeof(Join))
            {
                return tinf + "." + name;
            }
            else
            {
                return ((Join)obj1).getField(name);
            }
        }
    }
}