using System;

#if UNITY_IOS
namespace SkillzSDK.Internal.API.iOS
{
	internal sealed class MonoPInvokeCallbackAttribute : Attribute
	{
#pragma warning disable 414
		private readonly Type type;

		public MonoPInvokeCallbackAttribute(Type t)
		{
			type = t;
		}
#pragma warning restore 414
	}
}
#endif