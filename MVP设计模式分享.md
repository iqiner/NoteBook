Sure.J.Deng
16/5/23

### MVP设计模式——初探
#### 使用场景

提到MV*模式我们会想到的就是MVC，MVP，MVVM三种。他们存在的一个目标，其实只有一个：解耦UI逻辑和业务逻辑。它们各自都有不同的应用领域。一般来说：
- Web应用 》MV
- Winform 》 MVP
- WPF 》MVVM

今天不谈这几种模式的差异。毕竟没有实践就没有发言权。  今天要谈的就是MVP在WinForm上应用的一些不成熟的小建议，我自己也只是小小的研究了一下，今天算是给大家Share一下MVP编程上的一些心得体会吧。

#### MVP的优点【它会给我们带来什么好处】
因为我们Shipping组里维护的主要程序Shipping和SOProcess就是两个WinForm程序。基本上这两个程序都谈不上什么良好的编程模型。就是最原始的CodeBehinde模式。

UI逻辑和业务逻辑没有分开，导致的首要问题是业务逻辑没有复用或是很少复用。在程序里面，稳定性最高的就是业务逻辑，稳定性最差的就是UI。当这两个搅在一起之后，决定整个程序的稳定性的就是其中的短板—UI的稳定性。

第二个问题就是没有办法做单元测试。虽然现有的程序已经对一些业务做了抽象，但是感觉抽象力度。现在的程序就是三层架构，View依赖BLL，BLL依赖DAL。 

我们常常在说要面向接口编程。按照面向 接口编程的要求，高层不应该依赖底层，而是都应该依赖接口。

面向接口的一个好处，就是我们在单元测试的时候可以将外部依赖抽象成接口，运用单元测试的工具进行Mock。

这样就解决了我们程序程序要实施单元测试的另一个障碍——外部依赖。因为外部依赖比如数据库依赖会导致我们的单元测试不可复用，也就是测试结果不能满足幂等性。

单元测出是另外一个话题，今天也不展开来分享，下次单独找个时间，我们来讨论。 

#### 引子
所有的设计模式的目的都是一些通用的编程原则的具体实现。比如高内聚，低耦合，比如面向接口编程，比如职责单一，比如最少知识原则。

在我们编程的过程中，我们首先会想到的就是职责单一，也就是将各个业务抽象出来，一个业务对象完成某一个功能。在所谓的三层或者N层架构中，这些业务对象就是BLL中的对象。UI层会直接依赖BLL层。

但这会导致某些不稳定。比如某一项功能是由一系列的业务对象来组合完成的，这构成了一个业务流程。如果UI层直接依赖BLL层，那UI就承担了控制业务流程的职责，
```c#
private void ShipBtn_Click(object sender, EventArgs args)
{
    Biz1();
    Biz2();
    Biz3();    
}
```
一旦当业务流程发生改变，本来UI层是不应该受到影响，但是由于UI还承担了控制流程的作用，这个时候UI层也会受到影响。而且由于这个流程的控制在UI层， 单元测试也没有办法来测试。因为每一个单元测试用例的设计，应该是针对一个行为，一个行为就是一个业务。比如Ship这个行为就包含一系列流程。

那这个时候，我们怎么办？当然是将这个这个业务流程封装起来，形成一个流程控制层，UI层直接依赖这个控制层就可以解决以上说的这种不稳定。

但是这并不符合面向接口编程的精神。那接下来，我们将View抽象成接口。
```c#
public interface IView
{
    event Action<Shipment> OnShiped;
    
    void UpdateShipment(Shipment shipment);
}

public class View  : IView
{
    public event Action<Order> OnShiped{get; set;}
    
    private Shipment CurrentShipment{get;set;}   
    
    private void ShipBtn_Click(object sender, EventArgs args)
    {
        if(this.OnShiped != null)
        {
            this.OnShiped(this.CurrentShipment);    
        } 
    }
    
    public void UpdateShipment(Shipment shipment)
    {
        this.CurrentShipment = shipment;
        //TODO: Biding Data to UI     
    }
}

public class Controller 
{
    public IView m_View;
    
    publi Control(IView view)
    {
        this.m_View = view；
    }
    
    private void OnShiped(Shipment shipment)
    {
        this.CheckShipmentBeforeShipped(shipment);
        
        Modele1.Biz1(shipment);
        Modeile2.Biz2(shipment);
        Model3.Biz3(shipment);
        
        this.m_View.UpdateShipment(shipment);
    }
    
    private bool CheckShipmentBeforeShipped(shipment)
    {
        
    }
}
``` 
到这一步，已经通过控制层完全解耦了UI层和和业务层。其实这里的控制层就是MVP中的Presenter，业务层就是MVP中Model。

#### MVP架构
![MVP架构图](http://images.cnblogs.com/cnblogs_com/artech/201203/201203082136518106.png)

##### Model

Model是业务的抽象，本着职责单一的原则一个Model应该是一个业务的抽象。具有内聚性和自治性。

##### Presenter
Presenter是一个整个应该的中枢，它在驱动着整个流程的进行。

当用户在View上提交了某个请求，View会将这个请求转发给Presenter。Presenter拿到这个请求后去调用响应的Model，完成后将结果响应给View。

它不负责View的事情，也不具体负责某个Model的事情，它只是协调View和Model。该View做的事情，还是View做，比如数据的绑定，该Model完成的还是Model来完成，它只是执行调用。

但是以前UI上的一些会被转成到Presenter上来完成，比如数据的验证逻辑，比如有的时候，我们从Model拿到的数据不能满足View的数据格式，我们做的一些适配的逻辑。

##### View
View也就是终端用户交互的UI。它负责的东西是与用户交互，接受用户的请求，并将来自的用户的请求转发给Presenter，并等待Presenter的响应，然后将这个响应反馈给用户。

这里说的等待Presenter的响应注意不是指的从Presenter去拉去结果，而是等待Presenter主动推送结果过来。

因为Pull的动作产生的一个副作用就是View会强依赖Presenter，并且最终会MVP这个P是不是Presenter，而是变成了一个Proxy，一个对Model的代理。

正如第2点说的Presenter是整个程序的中枢，是老大。既然是老大管的事情必然不会太细，它只是下达指令。

#### 特别说明

    以上的阐述可能有些抽象，就拿我们OZZO的对美服务来举个形象的例子来说明这个架构：
    想一想我们的工作流程是怎么开展的？是不是经过了以下步骤：
    1. 仓库工作人员给BSD提出需求
    2. BSD将需求转发给我们OZZO
    3. OZZO的领导，比如John会负责接收这个需求，进行分析。然后经过分析，分解后的任务，分发给响应的开发和测试，比如我、Niki、Angel
    4. 开发和测试完成任务后，John就要Release项目。
    5. BSD Launch项目
    
    我们的系统，就是仓库里的工人在使用，那么个工人就是我们的用户，BSD其实就是充当了OZZO的前端View。
    
    这个View拿到用户的请求，他并不决定由哪个开发来完成，因为他并不了解开发。这里开发就相当于MVP中的Model，开发才是完成这个请求的具体实施者。
    
    BSD和要做的事情就是把这个需求转发给John. John就相当于Presenter，它是这个流程的中枢，起到的是承上启下的作用，John经过分析再将任务分发给开发（Model）。
    
    开发完成后，John Release项目就是Presenter将Model的处理结果响应给View（BSD）。
    
    BSD Launch项目的过程就相当于View进行数据绑定将结果呈现给用户的过程。
        
#### MVP规范
    1. 所有的View都应该以View后缀结束：TaskView/ITaskView
    2. 所有的Presenter都应该以Presenter后缀结束：TaskViewPresenter
    3. 让Presenter来处理所有请求，View仅处理GUI的操控细节。换句话说，就是View不处理具体的业务，应该交由Presenter来处理，View仅仅响应用户操作，以及完成数据绑定。
    4. View对Presenter方法的调用应该像事件触发一样：OnXxx（）
    5. View对Presenter的调用应该尽量少，而且只能是第4点的调用方式
    6. 禁止通过Presenter直接访问Model或者Service取得结果，Presenter的方法都应该是无返回值的。这样可以保证View不是从Presenter拉去数据，而是有Presenter主动将结果推送给View。
    7. Presenter对View的调用，应该仅通过接口。每个View都应该对应一个IView, Presenter只依赖IView，不依赖实现。这样可以在单元测试的时候Mock View。
    8. View中除了IView中定义的方法之外都应该是非Public的。
    9. 除了Presenter，禁止从其他的地方直接访问View。
    10. IView中定义的方法应该根据use-case取一个具有业务含义的名称，像SetDataSource这样意义模糊的命名是不合适的。
    11. IView中不要只定义方法，不应该定义属性。Presenter对View调用应该通过方法的调用，而不是Set Data。
    12. 数据应该保持在Model中。
    13. View中方法名不应该包含具体的控件名，这会导致Presenter知道太多的View的实现细节。
    14. View中的方法名应该具有业务含义，这样代码更容易被理解和自描述。

#### 实战演练

#### 讨论
    


  	
