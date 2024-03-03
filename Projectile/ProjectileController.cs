using System.Linq;

public partial class ProjectileController : ResourceController<ProjectileInfoCollection, ProjectileInfo>
{
    public static ProjectileController Instance => GetController<ProjectileController>("Projectile");
    public ProjectileInfoCollection Collection => GetCollection(ResourcePaths.Instance.Collection.ProjectileInfoCollection);

    public ProjectileInfo GetInfo(ProjectileType type)
    {
        var info = Collection.Resources.FirstOrDefault(x => x.Type == type);

        if (info == null)
        {
            Debug.LogError($"Found no ProjectileInfo with type: {type}");
            return null;
        }

        return info;
    }

    public Projectile CreateProjectile(ProjectileType type)
    {
        var info = GetInfo(type);
        var projectile = GDHelper.Instantiate<Projectile>(info.Scene);
        return projectile;
    }
}
