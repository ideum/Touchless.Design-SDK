using System;

namespace TouchlessDesign {

  public delegate void PropertyChangedDelegate<T>(Property<T> property, T oldValue, T value);
  
  public class Property<T> {

    public event PropertyChangedDelegate<T> Changed;

    public Property() { }

    public Property(T value) {
      _value = value;
    }

    public Property(Func<T> getter, Func<T, bool> setter) {
      _getter = getter;
      _setter = setter;
    }

    public Property(params PropertyChangedDelegate<T>[] listeners) {
      foreach (var t in listeners) {
        Changed += t;
      }
    }

    public Property(T value, params PropertyChangedDelegate<T>[] listeners) {
      foreach (var t in listeners) {
        Changed += t;
      }
      Value = value;
    }

    private T _value;
    private Func<T> _getter;
    private Func<T, bool> _setter;

    public bool Notify { get; set; } = true;

    public T Value {
      get {
        if (_getter != null) {
          return _getter();
        }
        return _value;
      }
      set {
        var oldValue = Value;
        if (_setter!=null) {
          if (_setter(value) && Notify) {
            Changed?.Invoke(this, oldValue, value);
          }
        }
        else {
          _value = value;
          if (Notify) {
            Changed?.Invoke(this, oldValue, value);
          }
        }
      }
    }

    public Property<T> SetSetter(Func<T, bool> setter) {
      _setter = setter;
      return this;
    }

    public Property<T> SetGetter(Func<T> getter) {
      _getter = getter;
      return this;
    }

    public Property<T> SetValue(T newValue) {
      Value = newValue;
      return this;
    }

    public Property<T> AddChangedListener(PropertyChangedDelegate<T> listener, bool invoke = false) {
      Changed += listener;
      if (invoke) {
        listener(this, Value, Value);
      }
      return this;
    }

    public Property<T> RemoveChangedListener(PropertyChangedDelegate<T> listener) {
      Changed -= listener;
      return this;
    }

    public Property<T> InvokeOn(PropertyChangedDelegate<T> listener) {
      listener(this, Value, Value);
      return this;
    }

    public Property<T> Invoke() {
      if (!Notify) return this;
      Changed?.Invoke(this, Value, Value);
      return this;
    }
  }
}