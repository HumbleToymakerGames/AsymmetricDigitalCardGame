
public class Subroutine_LoseClicks : Subroutine
{
    public int numClicks;
    public PlayerSide targetPlayerSide;

	public override void Fire()
	{
		base.Fire();
        PlayerNR.GetPlayer(targetPlayerSide).ActionPointsUsed(numClicks);
	}

}
