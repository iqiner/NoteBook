//函数里this指针指向谁？一句话，谁在调用我，我就指向谁！

//普通function
function testNormalFunction{
    console.log(this.name);
}
//执行函数
testNormalFunction();
//结果：undefined

//定义name
var name = "sure.j.deng";
testNormalFunction();
//结果：sure.j.deng
//总结：这个例子说明，这个this指向的是window对象
//每个function的原型（prototype）上都有一个call方法。这个方法传入的第一个参数就是this的指向。
//其实testNormalFunction(); =》 testNormalFunction.call(window);
testNormalFunction.call(window);
//实际上可以理解为testNormalFunction()这个方式是在被window调用， 所以this指向window


var person = {
    firstName : "sure",
    lastName : "deng",
    fullName : function(){
        return this.fullName + " " + this.lastName;
    }
};

person.fullName();
// sure deng
person.fullName.call(window);
//fullName是被person调用，所以this执行person


function hello(thing) {  
  console.log(this + " says hello " + thing);
}

person = { name: "Brendan Eich" }  
person.hello = hello;

person.hello("world") // still desugars to person.hello.call(person, "world")

hello("world") // "[object DOMWindow]world"  

var proxyFn = person.hello;
proxyFn("world") // "[object DOMWindow]world"




