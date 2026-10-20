using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class EntityIdPair
{
    public int ID {get; private set;}
    public DomainEntityType EntityType {get; private set;}

    public EntityIdPair(int id, DomainEntityType entityType)
    {
        ID = id;
        EntityType = entityType;
    }

    private EntityIdPair()
    {
        
    }
}
