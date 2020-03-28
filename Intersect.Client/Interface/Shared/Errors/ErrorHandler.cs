using System.Collections.Generic;

using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared.Errors
{

    public class ErrorHandler
    {

        //Controls
        private List<ErrorWindow> mErrors = new List<ErrorWindow>();

        //Canvasses
        private Canvas mGameCanvas;

        private Canvas mMenuCanvas;

        //Init
        public ErrorHandler(Canvas menuCanvas, Canvas gameCanvas)
        {
            mGameCanvas = gameCanvas;
            mMenuCanvas = menuCanvas;
        }

        public void Update()
        {
            if (Interface.MsgboxErrors.Count > 0)
            {
                mErrors.Add(
                    new ErrorWindow(
                        mGameCanvas, mMenuCanvas, Interface.MsgboxErrors[0].Value,
                        !string.IsNullOrEmpty(Interface.MsgboxErrors[0].Key)
                            ? Interface.MsgboxErrors[0].Key
                            : Strings.Errors.title.ToString()
                    )
                );

                Interface.MsgboxErrors.RemoveAt(0);
            }

            for (var i = 0; i < mErrors.Count; i++)
            {
                if (!mErrors[i].Update())
                {
                    mErrors.RemoveAt(i);
                }
            }
        }

    }

}
