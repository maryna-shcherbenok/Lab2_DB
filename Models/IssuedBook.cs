using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models;

public partial class IssuedBook
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long RequestId { get; set; }

    public long BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
