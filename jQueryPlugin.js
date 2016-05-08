var popup = (function($) {
    var tasks = [];
    var pageLoad = function (tasks) {
            bindingEvent();
    }
        
    var refreshTaskList = function (tasks) {
        bindingEvent();
    }
        
    var bindingEvent = function()
    {
        
    }     
    
    var methods = {
        refresh : function (tasks) {
            refreshTaskList(tasks);
        }
    };
    
    $.ready(function(){
        pageLoad();
    });
    
    return methods;
})(jQuery);

popup.load();
popup.refresh();