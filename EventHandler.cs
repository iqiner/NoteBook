public class EventHandlerWrapper
{
    public MethodInfo Method{get; private set;}
    
    public object Target{get; private set;}
    
    public EventHandler Handler{get; private set;}
    
    public EventHandlerWrapper(EventHandler hander)
    {
        this.Method = hander.Method;
        this.Target = hander.Target;
        this.Handler += this.Invoke;
    }
    
    private void Invoke(object sender, EventArgs args)
    {
        try
        {   
            this.Method.Invoke(this.Target, new object[]{sender, args});
        }
        catch (TargetInvocationException ex)
        {
            Excetpion innerEx = ex.InnerExeption;
            Log(ex); 
            MessageBox.Show(ex.Message);   
        }
    }
    
    public static implicit operator EventHandler(EventHandlerWrapper handler)
    {
        return hander.Handler;
    }
}