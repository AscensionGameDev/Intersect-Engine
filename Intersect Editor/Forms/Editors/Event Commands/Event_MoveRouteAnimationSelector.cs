/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class Event_MoveRouteAnimationSelector : UserControl
    {
        private Event_MoveRouteDesigner _routeDesigner;
        private MoveRouteAction _myAction;
        private bool _newAction = false;
        public Event_MoveRouteAnimationSelector(Event_MoveRouteDesigner moveRouteDesigner,MoveRouteAction action, bool newAction = false)
        {
            InitializeComponent();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general","none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            if (!newAction)
            {
                cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation,action.AnimationIndex);
            }
            _newAction = newAction;
            _routeDesigner = moveRouteDesigner;
            _myAction = action;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpSetAnimation.Text = Strings.Get("eventsetanimation", "title");
            lblAnimation.Text = Strings.Get("eventsetanimation", "label");
            btnOkay.Text = Strings.Get("eventsetanimation", "okay");
            btnCancel.Text = Strings.Get("eventsetanimation", "cancel");
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            _myAction.AnimationIndex = Database.GameObjectIdFromList(GameObject.Animation,cmbAnimation.SelectedIndex);
            _routeDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _routeDesigner.RemoveLastAction();
            _routeDesigner.Controls.Remove(this);
        }
    }
}
