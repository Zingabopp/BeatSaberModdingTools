using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;


namespace $rootnamespace$
{
	internal class $safeitemrootname$ : BSMLResourceViewController
	{
		// For this method of setting the ResourceName, this class must be the first class in the file.
		internal override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);
	}
}
