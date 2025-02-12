using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
  int a, b = Console.ReadLine();
  a = (3 + 5) * 8 - (10 - 2 * b) / b; // 61
  a--;
  a += 40;
  a *= 2;
  a--;
  a -= 99;

  int n = 5;

  for (b = 100; a < n; a++)
  {
    b++;
    while (b != 5)
    {
      if (n == 5)
      {
        Console.WriteLine("n es igual a 5");
      }
      else
      {
        Console.WriteLine("n es diferente de 5");
      }
    }
  }
  a++;
  if (a % 2 == 0)
  {
    Console.WriteLine("Es par " + a + " " + b);
    if (b == 3)
    {
      Console.WriteLine("b es igual a 2");

      if (b > 3)
      {
        Console.WriteLine("b es mayor a 3");
      }
    }
    else
    {
      Console.WriteLine("b no es igual a 3");
    }
  }
  else
  {
    Console.WriteLine("Es impar");
  }
}