using System.Runtime.CompilerServices;

namespace HlifeApi.Services
{
    public class CountService
    {
        private Dictionary<string, int> Dict = new Dictionary<string, int>();

        public int GetCount([CallerFilePath] string caller = "")
        {
            if (Dict.ContainsKey(caller))
            {
                return Dict[caller];
            }
            else
            {
                Dict.Add(caller, 0);
                return 0;
            }
        }

        public void AddCount(int i = 1, [CallerFilePath] string caller = "")
        {
            if (Dict.ContainsKey(caller))
            {
                Dict[caller] += i;
            }
            else
            {
                Dict.Add(caller, i);
            }
        }
    }
}