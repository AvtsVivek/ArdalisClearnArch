using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftDelete.SharedKernel;

public interface ISoftDelete
{
  bool IsSoftDeleted { get; set; }
}
