using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
  int a, b = Console.ReadLine();
  a = (3 + 5) * 8 - (10 - 2*b) / b; // 61

  int c = 0;
  c++;
  c++;
  c /= 2;

  if(a + 9 == 70) {
    Console.WriteLine("Es verdadero");
    if(b == 2) {
      Console.WriteLine("b es igual a 2");

      if(b > 3) {
        Console.WriteLine("b es mayor a 3");
      }
    }
  }
}