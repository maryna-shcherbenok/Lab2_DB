using System;
using System.Collections.Generic;

namespace Lab2_DB.Models;

public partial class IssuedBook
{
    public long Id { get; set; }

    public long RequestId { get; set; }

    public long BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
