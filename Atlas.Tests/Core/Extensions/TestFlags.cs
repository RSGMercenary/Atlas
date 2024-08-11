namespace Atlas.Tests.Core.Extensions;

[Flags]
internal enum TestFlags
{
	None = 0,
	A = 1,
	B = 2,
	C = 4,
	D = 8,
	E = 16,
	F = 32,
	G = 64,
	H = 128,
	I = 256,
	J = 512,
	K = A | J,
	L = B | J,
	M = C | J,
	N = D | J,
	O = E | J,
}