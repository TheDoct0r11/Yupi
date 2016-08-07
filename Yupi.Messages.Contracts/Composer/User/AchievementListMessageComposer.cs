using System.Collections.Generic;
using Yupi.Protocol.Buffers;
using Yupi.Model.Domain;

namespace Yupi.Messages.Contracts
{
	public abstract class AchievementListMessageComposer : AbstractComposer<UserInfo, List<Achievement>>
	{
		public override void Compose(Yupi.Protocol.ISender session, UserInfo user, List<Achievement> achievements)
		{
		 // Do nothing by default.
		}
	}
}