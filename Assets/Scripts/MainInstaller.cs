using System.ComponentModel;
using PlayerSystem;
using States;
using UnityEngine.InputSystem;
using Zenject;

public class MainInstaller:MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<PlayerMovementState>().AsSingle();
        Container.Bind<PlayerClimbingState>().AsSingle();
        Container.Bind<PlayerShootingState>().AsSingle();
    }
}