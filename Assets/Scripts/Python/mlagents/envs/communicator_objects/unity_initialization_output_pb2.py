# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: mlagents/envs/communicator_objects/unity_initialization_output.proto

from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from mlagents.envs.communicator_objects import brain_parameters_pb2 as mlagents_dot_envs_dot_communicator__objects_dot_brain__parameters__pb2
from mlagents.envs.communicator_objects import environment_parameters_pb2 as mlagents_dot_envs_dot_communicator__objects_dot_environment__parameters__pb2


DESCRIPTOR = _descriptor.FileDescriptor(
  name='mlagents/envs/communicator_objects/unity_initialization_output.proto',
  package='communicator_objects',
  syntax='proto3',
  serialized_options=b'\252\002\034MLAgents.CommunicatorObjects',
  serialized_pb=b'\nDmlagents/envs/communicator_objects/unity_initialization_output.proto\x12\x14\x63ommunicator_objects\x1a\x39mlagents/envs/communicator_objects/brain_parameters.proto\x1a?mlagents/envs/communicator_objects/environment_parameters.proto\"\xc6\x01\n\x1eUnityInitializationOutputProto\x12\x0c\n\x04name\x18\x01 \x01(\t\x12\x44\n\x10\x62rain_parameters\x18\x02 \x03(\x0b\x32*.communicator_objects.BrainParametersProto\x12P\n\x16\x65nvironment_parameters\x18\x03 \x01(\x0b\x32\x30.communicator_objects.EnvironmentParametersProtoB\x1f\xaa\x02\x1cMLAgents.CommunicatorObjectsb\x06proto3'
  ,
  dependencies=[mlagents_dot_envs_dot_communicator__objects_dot_brain__parameters__pb2.DESCRIPTOR,mlagents_dot_envs_dot_communicator__objects_dot_environment__parameters__pb2.DESCRIPTOR,])




_UNITYINITIALIZATIONOUTPUTPROTO = _descriptor.Descriptor(
  name='UnityInitializationOutputProto',
  full_name='communicator_objects.UnityInitializationOutputProto',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='name', full_name='communicator_objects.UnityInitializationOutputProto.name', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='brain_parameters', full_name='communicator_objects.UnityInitializationOutputProto.brain_parameters', index=1,
      number=2, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='environment_parameters', full_name='communicator_objects.UnityInitializationOutputProto.environment_parameters', index=2,
      number=3, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=219,
  serialized_end=417,
)

_UNITYINITIALIZATIONOUTPUTPROTO.fields_by_name['brain_parameters'].message_type = mlagents_dot_envs_dot_communicator__objects_dot_brain__parameters__pb2._BRAINPARAMETERSPROTO
_UNITYINITIALIZATIONOUTPUTPROTO.fields_by_name['environment_parameters'].message_type = mlagents_dot_envs_dot_communicator__objects_dot_environment__parameters__pb2._ENVIRONMENTPARAMETERSPROTO
DESCRIPTOR.message_types_by_name['UnityInitializationOutputProto'] = _UNITYINITIALIZATIONOUTPUTPROTO
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

UnityInitializationOutputProto = _reflection.GeneratedProtocolMessageType('UnityInitializationOutputProto', (_message.Message,), {
  'DESCRIPTOR' : _UNITYINITIALIZATIONOUTPUTPROTO,
  '__module__' : 'mlagents.envs.communicator_objects.unity_initialization_output_pb2'
  # @@protoc_insertion_point(class_scope:communicator_objects.UnityInitializationOutputProto)
  })
_sym_db.RegisterMessage(UnityInitializationOutputProto)


DESCRIPTOR._options = None
# @@protoc_insertion_point(module_scope)