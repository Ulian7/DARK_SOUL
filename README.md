# Unity Game - Dark Soul 实践

### 1. Movement

#### Input System

![](Images/InputSystem.png)

#### Workflow

![](Images/Workflow-PlayerInput.png)

Action Asset：An asset type which contains a saved configuration of Action Maps, Actions and Bindings，能够为不同的场景（步行或载具）创建Action Map，为不同的设备创建Control Scheme

### 2. Camera Handler

#### 相机更随

```c#
Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
myTransform.position = targetPosition;
```

#### 视角转动

![](Images/视角转动.png) 

Camera Holder控制绕 **Y** 轴方向的转动，Camera Pivot控制绕 **Local X** 轴方向的转动。

#### 执行顺序

 ![](Images/优先级设置.png)

![](Images/脚本优先级.png) 

因为InputHandler需要用到CameraHandler的单例，所以CameraHandler Script的执行顺序要先于InputHandler Script

### 3. Cinemachine

改用Cinemachine了（滑稽）

#### FreeLook

为Top、Middle和Bottom设置了三个旋转轨道，高度在范围内的相机的位置会自动混合

通过修改`m_YAxis.Value` （0~1）来调整镜头的高度，通过修改`Follow`对象的`Rotation`属性来保证虚拟相机能够初始化在`player`的正后方。

#### Cinemachine Collider

可以自动调节相机的位置以避免遮挡

#### 类魂镜头实现：

用实现了一个延迟移动的鬼影，将FreeLook的Follow和Aim都所在了鬼影上

```C#
void Update()
{
	Vector3 position = Vector3.Lerp(this.transform.position, player.position, speed * Time.deltaTime);
	this.transform.position = position;
}
```

### 4. Root Motion

#### **Apply Root Motion**

当你使用的骨骼动画会使得整个对象发生位移或偏转的时候，勾选Animator下的Apply Root Motion选项，会将位移和偏转实时附加到父物体上。

#### **Bake into Pose**

将整个骨骼动画对角色产生的位移和偏转，转化为姿势，或者说BodyPose，接下来无论你是否勾选Apply Root Motion，都将不会使得父级的Transform发生变化。（直到你勾选Apply Root Motion并使用第二个动画为止）



**使用Ctrl+d 从fbx文件中提取anim动画**

### 5. Function Structure

![](Images/Function_structure.png) 

### 6. Falling and Landing

#### Character Controller

使用SimpleMove来处理玩家的移动

将重力调为原来的2倍避免位移过度

#### 处理相关的动画

```C#
 if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeedToBeginFall, ignoreForGroundCheck))
 {
     playerManager.isGrounded = true;

     if (playerManager.isInAir)
     {
         if(inAirTimer > 0.5)
         {
             animatorHandler.PlayTargetAnimation("Land", true);
         }
         else
         {
             //animatorHandler.PlayTargetAnimation("Empty", false);
             inAirTimer = 0;
         }
         playerManager.isInAir = false;
     }
 }
 else
 {
     if (playerManager.isGrounded)
     {
         playerManager.isGrounded = false;
     }

     if (playerManager.isInAir == false)
     {
         if (playerManager.isInteracting == false)
         {
             animatorHandler.PlayTargetAnimation("Falling", true);
         }

         playerManager.isInAir = true;
     }
 }
```

### 7. Weapon

#### ScriptableObject

- ScriptableObject的数据是存储在asset里的，因此它不会在退出时被重置数据

- 这些资源在实例化的时候是可以被引用，而非复制

- 类似其他资源，它可以被任何场景引用，即场景间共享

- 在项目之间共享

##### 生命周期

- 当它是被绑定到.asset文件或者AssetBundle等资源文件中的时候，它就是persistent的
  - 它可以通过Resources.UnloadUnusedAssets来被unload出内存
  - 可以通过脚本引用或其他需要的时候被再次load到内存

- 如果是通过CreateInstance<>来创建的，它就是非persistent的

  - 它可以通过GC被直接destroy掉（如果没有任何引用的话）

  - 如果不想被GC的话，可以使用HideFlags.HideAndDontSave

##### 应用场景

第一种最常见的就是数据对象和表格数据，我们可以在Assets下创建一个.asset文件，并在编辑器面板中编辑修改它，再提交这个唯一的一份资源给版本控制器。例如，本地化数据、清单目录、表格、敌人配置等（这些真的非常常见，目前我接触过的大部分都是通过json、xml文件或是Monobehaviour来实现的，json和xml文件对策划并不友好）。

使用ScriptableObject的一个好处是你不需要考虑序列化的问题，但是我们也可以和Json这些进行配合（使用JsonUtility），既支持直接在编辑器里创建ScriptableObject，也支持在运行时刻通过读取Json文件来创建。例子是，内置 + 用户自定义的场景文件，我们可以在编辑器里设计一些场景存储成.asset文件，而在运行时刻玩家可以自己设计关卡存储在Json文件里，然后可以据此生成相应的ScriptableObject。

我们经常会需要一个可以在场景间共享的Singleton对象，有时候我们就可以使用ScriptableObject + static instance variable的方法来解决，当场景变换的时候，我们可以使用Resources.FindObjectsOfTypeAll<>来找到已有的instance（当然这需要在实例化第一个instance的时候把它标识为instance.hideFlags = HideFlags.HideAndDontSave）。

ScriptableObject除了可以存储数据外，我们还可以在ScriptableObject中定义一些方法，MonoBehaviour会把自身传递给ScriptableObject中的方法，然后ScriptableObject再进行一些工作。这类似于插槽设计模式，ScriptableObject提供一些槽，MonoBehaviour可以把自己插进去。适用于AI类型、加能量的buff或debuffs等。
```C#
abstract class PowerupEffect : ScriptableObject {
    public abstract void ApplyTo(GameObject go);
}

[CreateAssetMenu]
class HealthBooster : PowerupEffect {
    public int Amount;
    public override void ApplyTo(GameObject go) {
        go.GetComponent<Health>().currentValue += Amount;
    }
}

class Powerup : MonoBehaviour {
    public PowerupEffect effect;
    public void OnTriggerEnter(Collider other) {
        effect.ApplyTo(other.gameObject);
    }
}
```

我们先声明了一个PowerupEffect抽象类，来规定所有的加能量技能都需要定义一个ApplyTo函数作用于玩家。然后，我们定义一个HealthBooster类来管理那些专门加血的技能，我们可以通过创建资源的方式创建多个加血技能的资源实例，它们每个都可以有不同的加血量（Amount），当传进来一个GameObject的时候，就可以给它加血。我们又定义了Powerup的MonoBehaviour类，把它作为Component赋给各个可以触发加血技能的物体，它们可以接受一个PowerupEffect类型的加能量技能，然后靠碰撞体触发加血行为。

### 8. 受击检测

角色增加Capsule Collider，武器增加Box Collider，在攻击的动画中设置开启与关闭武器Collider的关键帧，并在动画末尾增加连击区间。

### 9. 输入缓冲

实现了翻滚和攻击的输入缓冲，允许玩家预输入一个指令。

采用枚举类型`next_state`来保存玩家预输入的指令

```C#
    public enum next_state 
    {
        Null,
        Roll,
        Attack,
        Item
    }
```



