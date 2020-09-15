public class Subroutine_EndTheRun : Subroutine
{

	public override void Fire()
	{
		base.Fire();
		RunOperator.instance.EndRun(false);
	}


}
