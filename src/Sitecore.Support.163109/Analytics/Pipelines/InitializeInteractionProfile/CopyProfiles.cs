using Sitecore.Analytics.Data;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using Sitecore.Analytics.Pipelines.InitializeInteractionProfile;

namespace Sitecore.Support.Analytics.Pipelines.InitializeInteractionProfile
{
    public class CopyProfiles : InitializeInteractionProfileProcessor
    {
        public override void Process(InitializeInteractionProfileArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Session, "args.Session");
            Contact contact = args.Session.Contact;
            CurrentInteraction interaction = args.Session.Interaction;
            if (contact != null && interaction != null)
            {
                BehaviorProfileConverterBase behaviorProfileConverterBase = BehaviorProfileConverterBase.Create();
                System.Collections.Generic.IList<ProfileData> list = new System.Collections.Generic.List<ProfileData>();
                foreach (IBehaviorProfileContext current in contact.BehaviorProfiles.Profiles)
                {
                    try
                    {
                        ProfileData item = behaviorProfileConverterBase.Convert(current);
                        list.Add(item);
                    }
                    catch (Sitecore.Exceptions.ItemNotFoundException)
                    {
                        Log.Warn("Sitecore.Support.163109 - the behavior profile contains a reference to a deleted profile", this);
                    }
                }
                interaction.Profiles.Initialize(list);


                //Cannot access set porperty (internal set), use reflexion instead
                //Commented line of original code
                //interaction.IsContactBehaviorProfileApplied = true;

                //Support code start
                System.Reflection.PropertyInfo propertyInfo = interaction.GetType().GetProperty("IsContactBehaviorProfileApplied");
                propertyInfo.SetValue(interaction, true, null);
                //Support code end
            }
        }
    }
}
