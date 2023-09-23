#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion

namespace ResetCurrent3DView
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;
            // Get the current view
            var curView = doc.ActiveView;

            try
            {
                using (Transaction t = new Transaction(doc, "Reset 3D View"))
                {
                    t.Start();

                    string successMessage = "Reset of 'View Template' and Hide 'Levels annotation'\nDONE!";
                    string warningMessage = "You must select a 3D View to reset the\nView Template and Hide Levels annotation";

                    if (curView.ViewType == ViewType.ThreeD)
                    {
                        // Debug message for a 3D view
                        Debug.Print(successMessage);

                        // Reset View Template to none
                        curView.ViewTemplateId = ElementId.InvalidElementId;

                        // Find the "Show Levels" annotation category
                        var catAnnotateLevelsSettings = doc.Settings.Categories
                            .Cast<Category>()
                            .FirstOrDefault(c => c.CategoryType == CategoryType.Annotation && c.Name.Contains("Levels"));

                        if (catAnnotateLevelsSettings != null)
                        {
                            // Hide the "Show Levels" annotation category
                            curView.SetCategoryHidden(catAnnotateLevelsSettings.Id, true);

                            // Commit the changes
                            doc.Regenerate();

                            TaskDialog.Show("Info", successMessage);
                        }
                        else
                        {
                            // Handle the case where the category is not found
                            TaskDialog.Show("Error", "Category 'Show Levels' not found.");
                        }
                    }
                    else
                    {
                        // Debug message for non-3D views
                        Debug.Print($"The current view is: {curView.ViewType}\n{warningMessage}");
                        TaskDialog.Show("Warning", warningMessage);
                    }

                    t.Commit(); // Commit the transaction
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            //string buttonInternalName = "btnCommand1";
            //string buttonTitle = "Button 1";
            string buttonInternalName = "btn_Reset3DView";
            string buttonTitle = "Reset 3D View";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }


    }
}
