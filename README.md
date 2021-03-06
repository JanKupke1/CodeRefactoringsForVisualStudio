
# Overview
I need some refractors as used at [Original]https://github.com/NeVeSpl/CodeRefactoringsForVisualStudio However, these should full fill the following points:

1. [work with SetProperty from Prism](#ConvertToFullPrismWpfProperty)
2. [shorten the property text](#ShortenThePropertyText)
3. [keep the DataAnnotations](#KeepTheDataAnnotations)
4. use the notation stored in Visual Studio for the private variables


### <a name="ConvertToFullPrismWpfProperty"></a>1. Convert To full prism property
From
```C#
      public string SomeProperty { get; set; }  
```

To

```C#
 private string _someProperty;

 public string SomeProperty
        {
            get
            {
                return _someProperty;
            }

            set
            {
                SetProperty(ref _someProperty, value);
            }
        }

```
It works.

### <a name="ShortenThePropertyText"></a>2. shorten the property text
I want to bring the Code from:
```C#
 private string _someProperty;

 public string SomeProperty
        {
            get
            {
                return _someProperty;
            }

            set
            {
                SetProperty(ref _someProperty, value);
            }
        }

```
To
```C#
   private string _someProperty;

   public string SomeProperty
        {
            get{return _someProperty;}
            set{SetProperty(ref _someProperty, value);}
        }

```
But at now, it does't work

### <a name="KeepTheDataAnnotations"></a>2. keep the data annotations

From
```C#
      [Required]
      public string SomeProperty { get; set; }  
```

To
```C#
   private string _someProperty;
   
   [Required]
   public string SomeProperty
        {
            get{return _someProperty;}
            set{SetProperty(ref _someProperty, value);}
        }

```
But at now, it does't work
