using TMPro;

public class ScoringUI : PlayArea_Spot
{

	public TextMeshProUGUI scoringText;

	protected override void Awake()
	{
		base.Awake();
		scoringText.text = "0";
	}

	private void OnEnable()
	{
		myPlayer.OnScoreChanged += MyPlayer_OnScoreChanged;
	}
	private void OnDisable()
	{
		myPlayer.OnScoreChanged -= MyPlayer_OnScoreChanged;
	}

	private void MyPlayer_OnScoreChanged()
	{
		scoringText.text = myPlayer.Score.ToString();
	}
}
