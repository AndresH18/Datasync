// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server.Filters;
using CommunityToolkit.Datasync.Server.Private;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CommunityToolkit.Datasync.Server;

/// <summary>
/// The base controller type for a datasync table controller.  This exposes a "CRUD" endpoint set
/// and a list endpoint that uses OData query options.
/// </summary>
/// <typeparam name="TEntity">The type of the entity exposed to the client.</typeparam>
[DatasyncController]
public partial class TableController<TEntity> : TableController<TEntity, string> where TEntity : class, ITableData<string> //: ODataController where TEntity : class, ITableData<string>
{
    #region Controller constructors
    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/>. The table options (such as repository,
    /// access control provider, etc.) are set via the upstream controller.
    /// </summary>
    public TableController()
        : this(new Repository<TEntity>(), new AccessControlProvider<TEntity>(), null, new TableControllerOptions())
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to use for this controller.</param>
    public TableController(IRepository<TEntity> repository)
        : this(repository, new AccessControlProvider<TEntity>(), null, new TableControllerOptions())
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to use for this controller.</param>
    /// <param name="model">The <see cref="IEdmModel"/> to use for OData interactions.</param>
    public TableController(IRepository<TEntity> repository, IEdmModel model)
        : this(repository, new AccessControlProvider<TEntity>(), model, new TableControllerOptions())
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to use for this controller.</param>
    /// <param name="accessControlProvider">The access control provider to use for this controller.</param>
    public TableController(IRepository<TEntity> repository, IAccessControlProvider<TEntity> accessControlProvider)
        : this(repository, accessControlProvider, null, new TableControllerOptions())
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to use for this controller.</param>
    /// <param name="options">The <see cref="TableControllerOptions"/> to use for configuring this controller.</param>
    public TableController(IRepository<TEntity> repository, TableControllerOptions options)
        : this(repository, new AccessControlProvider<TEntity>(), null, options)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to use for this controller.</param>
    /// <param name="accessControlProvider">The access control provider to use for this controller.</param>
    /// <param name="options">The <see cref="TableControllerOptions"/> to use for configuring this controller.</param>
    public TableController(IRepository<TEntity> repository, IAccessControlProvider<TEntity> accessControlProvider, TableControllerOptions options)
        : this(repository, accessControlProvider, null, options)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to use for this controller.</param>
    /// <param name="accessControlProvider">The access control provider to use for this controller.</param>
    /// <param name="model">The <see cref="IEdmModel"/> to use for OData interactions.</param>
    public TableController(IRepository<TEntity> repository, IAccessControlProvider<TEntity> accessControlProvider, IEdmModel model)
        : this(repository, accessControlProvider, model, new TableControllerOptions())
    {
    }
    #endregion

    /// <summary>
    /// Creates a new <see cref="TableController{TEntity}"/>.
    /// </summary>
    /// <param name="repository">The repository that will be used for data access operations.</param>
    /// <param name="accessControlProvider">The access control provider that will be used for authorizing requests.</param>
    /// <param name="model">The <see cref="IEdmModel"/> to use for OData interactions (instead of generating the model).</param>
    /// <param name="options">The options for this table controller.</param>
    public TableController(IRepository<TEntity> repository, IAccessControlProvider<TEntity> accessControlProvider, IEdmModel? model, TableControllerOptions options) : base(repository, accessControlProvider, model, options)
    {
        Repository = repository;
        AccessControlProvider = accessControlProvider;
        EdmModel = model ?? ModelCache.GetEdmModel(typeof(TEntity));
        Options = options;

        if (EdmModel.FindType(typeof(TEntity).FullName) is null)
        {
            throw new InvalidOperationException($"The entity type {typeof(TEntity).FullName} is not registered in the OData EdmModel.");
        }
    }
}
