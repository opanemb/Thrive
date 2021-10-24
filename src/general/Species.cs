﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Newtonsoft.Json;

/// <summary>
///   Class that represents a species. This is an abstract base for
///   use by all stage-specific species classes.
/// </summary>
[JsonObject(IsReference = true)]
[TypeConverter(typeof(ThriveTypeConverter))]
[JSONDynamicTypeAllowed]
[UseThriveConverter]
public abstract class Species : ICloneable
{
    /// <summary>
    ///   This is the amount of compounds cells of this type spawn with
    /// </summary>
    [JsonProperty]
    public readonly Dictionary<Compound, float> InitialCompounds = new Dictionary<Compound, float>();

    public string Genus;
    public string Epithet;

    public Color Colour = new Color(1, 1, 1);

    /// <summary>
    ///   This holds all behavioural values and defines how this species will behave in the environment.
    /// </summary>
    [JsonProperty]
    public BehaviourDictionary Behaviour = new BehaviourDictionary();

    public int Generation = 1;

    protected Species(uint id)
    {
        ID = id;
    }

    /// <summary>
    ///   Unique id of this species, used to identity this
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     In the previous version a string name was used to identify
    ///     species, but it was just the word species followed by a
    ///     sequential number, so now this is an actual number.
    ///   </para>
    /// </remarks>
    [JsonProperty]
    public uint ID { get; private set; }

    /// <summary>
    ///   This is the genome of the species
    /// </summary>
    public abstract string StringCode { get; set; }

    /// <summary>
    ///   When true this is the player species
    /// </summary>
    [JsonProperty]
    public bool PlayerSpecies { get; private set; }

    [JsonIgnore]
    public string FormattedName => Genus + " " + Epithet;

    [JsonIgnore]
    public string FormattedIdentifier => FormattedName + $" ({ID:n0})";

    /// <summary>
    ///   Repositions the structure of the species according to stage specific rules
    /// </summary>
    public abstract void RepositionToOrigin();

    /// <summary>
    ///   Apply properties from the mutation that are mutable
    /// </summary>
    public virtual void ApplyMutation(Species mutation)
    {
        InitialCompounds.Clear();

        foreach (var entry in mutation.InitialCompounds)
            InitialCompounds.Add(entry.Key, entry.Value);

        foreach (var entry in mutation.Behaviour)
            Behaviour[entry.Key] = entry.Value;

        Colour = mutation.Colour;

        // These don't mutate for a species
        // genus;
        // epithet;
    }

    /// <summary>
    ///   Makes this the player species. This is a method as this is an important change
    /// </summary>
    public void BecomePlayerSpecies()
    {
        PlayerSpecies = true;
    }

    /// <summary>
    ///   Gets info specific to the species for storing into a new container class.
    ///   Used for patch snapshots, but could be expanded
    /// </summary>
    /// <remarks>TODO: Check overlap with ClonePropertiesTo</remarks>
    public SpeciesInfo RecordSpeciesInfo()
    {
        return new SpeciesInfo
        {
            ID = ID,
        };
    }

    /// <summary>
    ///   Creates a cloned version of the species. This should only
    ///   really be used if you need to modify a species while
    ///   referring to the old data. In for example the Mutations
    ///   code.
    /// </summary>
    public abstract object Clone();

    public override string ToString()
    {
        return FormattedIdentifier;
    }

    /// <summary>
    ///   Helper for child classes to implement Clone
    /// </summary>
    protected void ClonePropertiesTo(Species species)
    {
        foreach (var entry in InitialCompounds)
            species.InitialCompounds[entry.Key] = entry.Value;

        foreach (var entry in Behaviour)
            species.Behaviour[entry.Key] = entry.Value;

        species.Genus = Genus;
        species.Epithet = Epithet;
        species.Colour = Colour;
        species.Generation = Generation;
        species.ID = ID;
        species.PlayerSpecies = PlayerSpecies;
    }
}
