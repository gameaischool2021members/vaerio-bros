using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataLayer.Interfaces
{
    public interface IGenericRequester
    {
        string Endpoint { get; }

        IEnumerable<Tout> GetObjects<Tin, Tmid, Tout>(Tin obj);
        Tout GetObject<Tin, Tmid, Tout>(Tin obj);

        Tout PostObjects<Tin, Tmid, Tout>(IEnumerable<Tin> obj);

        IEnumerator GetObject<Tout>(string path, Action<Tout> responseHandler = null);

        IEnumerator PostObject<Tin, Tout>(Tin bodyObj, string path, Action<Tout> responseHandler = null);
    }
}
