/*
Task: Sending Contact Notification Email

Description: Develop a custom workflow that sends a notification email to a contact in the CRM system.
When triggered, the workflow retrieves the contact's email address and full name.
If the email address is available, an email is composed with the sender as a system user, the recipient as the contact, and a designated system user in CC.
The email subject and content are predefined.
 */
using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace CustomWorkflow
{
      class Task1Activity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            // Get the workflow context and service objects
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            // Get the updated Contact record
            Entity contact = service.Retrieve("contact", workflowContext.PrimaryEntityId, new Microsoft.Xrm.Sdk.Query.ColumnSet("emailaddress1", "fullname"));
            // Check if the email address is not empty
            if (contact.Contains("emailaddress1"))
            {
                // string emailAddress = contact.GetAttributeValue<string>("emailaddress1");
                string contactName = contact.GetAttributeValue<string>("fullname");

                

                // Create "From" , "To" and "cc" Activity Party entities
                Entity fromActivityParty = new Entity("activityparty");
                fromActivityParty["partyid"] = new EntityReference("systemuser", workflowContext.UserId); // set from

                Entity toActivityParty = new Entity("activityparty");
                toActivityParty["partyid"] = new EntityReference("contact" ,workflowContext.PrimaryEntityId); // Set TO

                Entity ccActivityParty = new Entity("activityparty");
                ccActivityParty["partyid"] = new EntityReference("systemuser", Guid.Parse("67189041-c032-ee11-bdf4-000d3a0aabb1")); // Set cc




                // Create the email activity
                Entity email = new Entity("email");
                email["from"] = new Entity[] { fromActivityParty };
                email["to"] = new Entity[] { toActivityParty };
                email["cc"] = new Entity[] { ccActivityParty };
                email["subject"] = "Notification from CRM";
                email["description"] = $"Hello {contactName},\n\nThis is a notification from CRM.";

                // Send the email
                service.Create(email);
            }
        }
    }
}


