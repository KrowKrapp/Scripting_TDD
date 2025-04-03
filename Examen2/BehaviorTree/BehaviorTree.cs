using System;
using System.Threading;

abstract class Node
{
    public abstract bool Execute();
}

class Root : Node
{
    private Node child;
    public Root(Node child) { this.child = child; }
    public override bool Execute() { return child?.Execute() ?? false; }
}

abstract class Composite : Node
{
    protected Node[] children;
    public Composite(params Node[] children) { this.children = children; }
}

class Sequence : Composite
{
    public Sequence(params Node[] children) : base(children) { }
    public override bool Execute()
    {
        foreach (var child in children)
            if (!child.Execute()) return false;
        return true;
    }
}

class Selector : Composite
{
    private Func<bool> condition;
    public Selector(Func<bool> condition, params Node[] children) : base(children) { this.condition = condition; }
    public override bool Execute()
    {
        if (condition != null && !condition()) return false;
        foreach (var child in children)
            if (child.Execute()) return true;
        return false;
    }
}
class Task : Node
{
    private Func<bool> action;
    public Task(Func<bool> action) { this.action = action; }
    public override bool Execute() { return action(); }
}

// Variables de IA
static int posicionPersonaje = 0;
static int posicionObjetivo = 10;
static int distanciaValida = 3;
static int tiempoEspera = 1500;

static bool EvaluarDistancia() => Math.Abs(posicionPersonaje - posicionObjetivo) <= distanciaValida;
static bool MoverPersonaje()
{
    if (posicionPersonaje < posicionObjetivo) posicionPersonaje++;
    Console.WriteLine($"Posición actual: {posicionPersonaje}");
    return posicionPersonaje == posicionObjetivo;
}
static bool Esperar()
{
    Console.WriteLine("Esperando...");
    Thread.Sleep(tiempoEspera);
    return true;
}

// árbol
static void Main()
{
    var selectorDistancia = new Selector(EvaluarDistancia, new Task(MoverPersonaje));
    var selectorSinEvaluacion = new Selector(null, selectorDistancia);
    var secuencia = new Sequence(selectorSinEvaluacion, new Task(Esperar));
    var arbol = new Root(secuencia);

    while (!arbol.Execute()) { }
    Console.WriteLine("Objetivo alcanzado!");
}
