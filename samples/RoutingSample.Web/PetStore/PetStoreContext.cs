// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace PetStore
{
    public class PetStoreContext : DbContext
    {
        public PetStoreContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}