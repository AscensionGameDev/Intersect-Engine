using System;

namespace IntersectServer
{
	public class MovementState  {
		public float posX;
		public float posY;
		public float velocityX;
		public float velocityY;
		public float rotation;
		public long updateTick;

		public MovementState Clone() {
			MovementState temp = new MovementState();
			temp.posX = posX;
			temp.posY = posY;
			temp.velocityX = velocityX;
			temp.velocityY = velocityY;
			temp.rotation = rotation;
			temp.updateTick = updateTick;
			return temp;
		}

	}
}

