namespace AppName.Core
{
	internal static class AssemblyGlobal
	{
		public const string Company = "LSRetail";

		public const string ProductLine = "LSRetail";

		public const string Year = "2020";

		public const string AssemblyVersion = "1.0.0.*";

        public const string Copyright = Company + " - " + Year;

#if DEBUG
        public const string Configuration = "Debug";
#elif RELEASE
        public const string Configuration = "Release";
#else
        public const string Configuration = "Unkown";
#endif
	}
}
