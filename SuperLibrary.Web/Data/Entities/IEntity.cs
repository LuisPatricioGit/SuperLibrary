﻿namespace SuperLibrary.Web.Data.Entities;

public interface IEntity
{
    int Id { get; set; }

    bool WasDeleted { get; set; }
}