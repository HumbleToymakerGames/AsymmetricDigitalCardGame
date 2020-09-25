using UnityEngine;

public class TagWrapper
{
    public int Tags
	{	get; private set; }

    public TagWrapper(int _tags) { Tags = _tags; }


    public void SubtractTags(int _tags)
	{
		Tags = Mathf.Max(Tags - _tags, 0);
	}

    public void AddTags(int _tags)
	{
		Tags += _tags;
	}



}
