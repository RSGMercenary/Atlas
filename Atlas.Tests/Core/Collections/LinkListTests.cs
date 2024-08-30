using Atlas.Core.Collections.LinkList;
using Atlas.Tests.Testers.Utilities;
using NUnit.Framework;
using System;

namespace Atlas.Tests.Core.Collections;

[TestFixture]
class LinkListTests
{
	private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private Random Random = new();
	private LinkList<string> List;

	[SetUp]
	public void SetUp()
	{
		List = new LinkList<string>();
	}

	#region Add
	[Test]
	public void When_Add_One_Then_FirstAndLastEqual()
	{
		var value = RandomLetter();

		List.Add(value);

		Assert.That(List.First != null);
		Assert.That(List.Last != null);
		Assert.That(List.First == List.Last);
		Assert.That(List.First.Value == value);
		Assert.That(List[0] == value);
		Assert.That(List.Contains(value));
	}

	[Test]
	public void When_Add_Count_Then_CountEquals()
	{
		var count = 0;

		AddLetters();

		for(var node = List.First; node != null; node = node.Next)
			count++;

		Assert.That(List.Count == count);
	}

	[Test]
	public void When_Add_AtIndex_Then_Added([Values(0, 1, 5, 8, 12, 17, 25, 26)] int index)
	{
		var count = 0;
		var letter = "_";

		AddLetters();

		List.Add(letter, index);

		for(var node = List.First; node != null; node = node.Next)
			count++;

		Assert.That(Letters.Length + 1 == count);
		Assert.That(List[index] == letter);
		Assert.That(List.Contains(letter));
	}

	[TestCase(-1)]
	[TestCase(1)]
	public void When_Add_AtInvalidIndex_Then_NotAdded(int index)
	{
		var count = 0;
		var letter = "_";

		List.Add(letter, index);

		for(var node = List.First; node != null; node = node.Next)
			count++;

		Assert.That(List.Count == 0);
		Assert.That(count == 0);
		Assert.That(!List.Contains(letter));
	}
	#endregion

	#region Remove
	[Test]
	public void When_Remove_One_Then_FirstAndLastNull()
	{
		var value = RandomLetter();

		List.Add(value);
		List.Remove(value);

		Assert.That(List.First == null);
		Assert.That(List.Last == null);
		Assert.That(List.Count == 0);
		Assert.That(!List.Contains(value));
	}

	[Test]
	public void When_Remove_Then_NotRemoved()
	{
		var value = RandomLetter();

		List.Remove(value);
		Assert.That(!List.Remove(value));
		Assert.That(!List.Contains(value));
		Assert.That(List.First == null);
		Assert.That(List.Last == null);
	}

	[Test]
	public void When_RemoveAll_Then_CountZero()
	{
		var count = 0;

		AddLetters();

		List.RemoveAll();

		for(var node = List.First; node != null; node = node.Next)
			count++;

		Assert.That(count == 0);
		Assert.That(List.Count == 0);
	}

	[Test]
	public void When_Remove_AtIndex_Then_Removed([Values(1, 1, 5, 8, 12, 17, 24)] int index)
	{
		var count = 0;
		var letter = GetLetter(index);

		AddLetters();

		List.Remove(index);

		for(var node = List.First; node != null; node = node.Next)
			count++;

		Assert.That(count == Letters.Length - 1);
		Assert.That(List[index] != letter);
		Assert.That(!List.Contains(letter));
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_RemoveAll_Then_BoolExpected(bool addLetters)
	{
		if(addLetters)
			AddLetters();
		Assert.That(List.RemoveAll() == addLetters);
	}

	[TestCase(".")]
	[TestCase(null)]
	public void When_Remove_Then_FalseExpected(string letter)
	{
		Assert.That(!List.Remove(letter));
		Assert.That(List.Count == 0);
	}
	#endregion

	#region Get / Set
	[TestCase(-1)]
	[TestCase(1)]
	public void When_Get_AtInvalidIndex_Then_NullExpected(int index)
	{
		Assert.That(List.GetNode(index) == default);
	}

	[Test]
	[Repeat(20)]
	public void When_SetIndex_Then_IndexSet()
	{
		var letter = "_";
		var index = Random.Next(0, Letters.Length);

		AddLetters();

		var value = List[index];
		List[index] = letter;

		Assert.That(!List.Contains(value));
		Assert.That(List.Contains(letter));
		Assert.That(List[index] == letter);
	}
	#endregion

	#region Swap
	[Test]
	public void When_Swap_AsValue_Then_ValuesSwapped()
	{
		AddLetters();

		var index1 = Random.Next(Letters.Length);
		var index2 = Random.NextExclude(Letters.Length, index1);

		var value1 = List[index1];
		var value2 = List[index2];

		List.Swap(value1, value2);

		Assert.That(List[index1] == value2);
		Assert.That(List[index2] == value1);
	}

	[Test]
	public void When_Swap_AsIndex_Then_ValuesSwapped()
	{
		AddLetters();

		var index1 = Random.Next(Letters.Length);
		var index2 = Random.NextExclude(Letters.Length, index1);

		var value1 = List[index1];
		var value2 = List[index2];

		List.Swap(index1, index2);

		Assert.That(List[index1] == value2);
		Assert.That(List[index2] == value1);
	}
	#endregion

	[Test]
	public void When_Clone_Then_Cloned()
	{
		AddLetters();

		Assert.That(List.ToString() == List.GetIterator().ToString());
	}

	[Test]
	public void When_ThingsHappen_ThenCry()
	{
		AddLetters();

		var iterator1 = List.GetIterator();
		var iterator2 = List.GetIterator();

		iterator2.Dispose();
		Assert.That(iterator1 != iterator2);
		Assert.That(iterator1.Count == 26);
	}

	private void AddLetters()
	{
		for(int i = 0; i < Letters.Length; i++)
			List.Add(GetLetter(i));
	}

	private string GetLetter(int index) => Letters[index].ToString();

	private string RandomLetter() => GetLetter(Random.Next(26));
}