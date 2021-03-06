﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Terraria.ID;

namespace FTWProtection
{
	[ApiVersion(2, 1)]
	public class FTWProtection : TerrariaPlugin
	{
		public override string Author => "Quinci";

		public override string Description => "Prevents explosion griefing in for the worthy";

		public override string Name => "For the Worthy Protection";

		public override Version Version => new Version(1, 0, 0, 0);

		public FTWProtection(Main game) : base(game)
		{
			Order = 40;
		}

		public override void Initialize()
		{
			ServerApi.Hooks.ProjectileSetDefaults.Register(this, OnProjectileSetDefaults);
			ServerApi.Hooks.ProjectileAIUpdate.Register(this, OnProjectileAIUpdate);
		}

		private void OnProjectileSetDefaults(SetDefaultsEventArgs<Projectile, int> args)
		{
			if (args.Object.type == ProjectileID.BombSkeletronPrime && Main.getGoodWorld) // 102 && ftw flag
			{
				args.Object.type = ProjectileID.BeeHive; // 655;
				args.Object.aiStyle = 25; // Beehive AI, rolls along the ground until crashing
				NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, args.Object.identity);
			}
		}

		private void OnProjectileAIUpdate(ProjectileAiUpdateEventArgs args)
		{
			// Only possible via modified client (where there are other issues) as x velocity will always be 5 if y = 0, regardless of tile collision.
			if (args.Projectile.type == ProjectileID.Bomb && args.Projectile.timeLeft == 3600 && args.Projectile.oldVelocity.Y == 0f && 0.202f >= Math.Abs(args.Projectile.oldVelocity.X))
			{
				args.Projectile.type = ProjectileID.BeeHive; // 655;
				args.Projectile.aiStyle = 25; // Beehive AI, rolls along the ground until crashing
				NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, args.Projectile.identity);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.ProjectileSetDefaults.Deregister(this, OnProjectileSetDefaults);
				ServerApi.Hooks.ProjectileAIUpdate.Deregister(this, OnProjectileAIUpdate);

			}
			base.Dispose(disposing);
		}
	}
}