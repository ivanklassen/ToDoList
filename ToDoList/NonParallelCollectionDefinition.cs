using Xunit;

namespace ToDoList
{
    [CollectionDefinition("NonParallelCollection", DisableParallelization = true)]
    public class NonParallelCollectionDefinition
    {
    }
}
