using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityModdingTools.Abstractions
{
    public static class Extensions
    {
        public static IEnumerable<IProjectElement> GetReferences(this IProjectModel model)
        {
            return model.Where(e => e is IItemGroup).SelectMany(e =>
            {
                IItemGroup itemGroup = (IItemGroup)e;
                return itemGroup.Where(i => i.Name == Names.Reference);
            });
        }

        public static IEnumerator<IProjectElement> EnumerateReferences(this IProjectModel model)
        {
            foreach (var element in model)
            {
                if(element is IItemGroup itemGroup)
                {
                    foreach (var item in itemGroup)
                    {
                        if (item.Name == Names.Reference)
                            yield return item;
                    }
                }
            }
        }

        public static IProjectComponent GetGameReferenceGroup(this IProjectModel model)
        {
            IProjectComponent? generalRefs = null;
            IProjectComponent? gameRefs = null;
            foreach (var element in model)
            {
                if (element is IItemGroup itemGroup)
                {
                    foreach (var item in itemGroup)
                    {
                        if (item.Name == Names.Reference)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }
    }
}
