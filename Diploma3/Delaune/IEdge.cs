﻿namespace Delaune
{
	public interface IEdge
	{
		IPoint P { get; }
		IPoint Q { get; }
		int Index { get; }
	}
}
