-keep class bitter.jnibridge.* { *; }
-keep class com.unity3d.player.* { *; }
-keep class org.fmod.* { *; }
-keep public class com.dylanvann.fastimage.* { *; }
-keep public class com.dylanvann.fastimage.** { *; }
-keepclassmembers enum * {
    public static **[] values();
    public static ** valueOf(java.lang.String);
}
-keep class retrofit.** {*;}
-keep class com.facebook.** {*;}
-keep class com.amazonaws.** {*;}