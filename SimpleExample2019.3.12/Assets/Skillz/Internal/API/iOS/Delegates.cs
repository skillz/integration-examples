using System;

#if UNITY_IOS
namespace SkillzSDK.Internal.API.iOS
{
	delegate void VoidFP();

	delegate void IntFP(ulong playerId); //This "UInt64" may need to be an "int"

	delegate void IntPtrIntFP(IntPtr value, ulong length); //This "UInt64" may need to be an "int"
}
#endif