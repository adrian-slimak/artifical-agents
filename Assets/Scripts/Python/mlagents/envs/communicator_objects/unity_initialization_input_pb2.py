# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: mlagents/envs/communicator_objects/unity_initialization_input.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from mlagents.envs.communicator_objects import engine_configuration_pb2 as mlagents_dot_envs_dot_communicator__objects_dot_engine__configuration__pb2


DESCRIPTOR = _descriptor.FileDescriptor(
  name='mlagents/envs/communicator_objects/unity_initialization_input.proto',
  package='communicator_objects',
  syntax='proto3',
  serialized_options=b'\252\002\034MLAgents.CommunicatorObjects',
  serialized_pb=b'\nCmlagents/envs/communicator_objects/unity_initialization_input.proto\x12\x14\x63ommunicator_objects\x1a=mlagents/envs/communicator_objects/engine_configuration.proto\"\xaa\x02\n\x1dUnityInitializationInputProto\x12\x0c\n\x04seed\x18\x01 \x01(\x05\x12L\n\x14\x65ngine_configuration\x18\x02 \x01(\x0b\x32..communicator_objects.EngineConfigurationProto\x12o\n\x17\x63ustom_reset_parameters\x18\x03 \x03(\x0b\x32N.communicator_objects.UnityInitializationInputProto.CustomResetParametersEntry\x1a<\n\x1a\x43ustomResetParametersEntry\x12\x0b\n\x03key\x18\x01 \x01(\t\x12\r\n\x05value\x18\x02 \x01(\x02:\x02\x38\x01\x42\x1f\xaa\x02\x1cMLAgents.CommunicatorObjectsb\x06proto3'
  ,
  dependencies=[mlagents_dot_envs_dot_communicator__objects_dot_engine__configuration__pb2.DESCRIPTOR,])




_UNITYINITIALIZATIONINPUTPROTO_CUSTOMRESETPARAMETERSENTRY = _descriptor.Descriptor(
  name='CustomResetParametersEntry',
  full_name='communicator_objects.UnityInitializationInputProto.CustomResetParametersEntry',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='key', full_name='communicator_objects.UnityInitializationInputProto.CustomResetParametersEntry.key', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value', full_name='communicator_objects.UnityInitializationInputProto.CustomResetParametersEntry.value', index=1,
      number=2, type=2, cpp_type=6, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=b'8\001',
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=395,
  serialized_end=455,
)

_UNITYINITIALIZATIONINPUTPROTO = _descriptor.Descriptor(
  name='UnityInitializationInputProto',
  full_name='communicator_objects.UnityInitializationInputProto',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='seed', full_name='communicator_objects.UnityInitializationInputProto.seed', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='engine_configuration', full_name='communicator_objects.UnityInitializationInputProto.engine_configuration', index=1,
      number=2, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='custom_reset_parameters', full_name='communicator_objects.UnityInitializationInputProto.custom_reset_parameters', index=2,
      number=3, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[_UNITYINITIALIZATIONINPUTPROTO_CUSTOMRESETPARAMETERSENTRY, ],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=157,
  serialized_end=455,
)

_UNITYINITIALIZATIONINPUTPROTO_CUSTOMRESETPARAMETERSENTRY.containing_type = _UNITYINITIALIZATIONINPUTPROTO
_UNITYINITIALIZATIONINPUTPROTO.fields_by_name['engine_configuration'].message_type = mlagents_dot_envs_dot_communicator__objects_dot_engine__configuration__pb2._ENGINECONFIGURATIONPROTO
_UNITYINITIALIZATIONINPUTPROTO.fields_by_name['custom_reset_parameters'].message_type = _UNITYINITIALIZATIONINPUTPROTO_CUSTOMRESETPARAMETERSENTRY
DESCRIPTOR.message_types_by_name['UnityInitializationInputProto'] = _UNITYINITIALIZATIONINPUTPROTO
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

UnityInitializationInputProto = _reflection.GeneratedProtocolMessageType('UnityInitializationInputProto', (_message.Message,), {

  'CustomResetParametersEntry' : _reflection.GeneratedProtocolMessageType('CustomResetParametersEntry', (_message.Message,), {
    'DESCRIPTOR' : _UNITYINITIALIZATIONINPUTPROTO_CUSTOMRESETPARAMETERSENTRY,
    '__module__' : 'mlagents.envs.communicator_objects.unity_initialization_input_pb2'
    # @@protoc_insertion_point(class_scope:communicator_objects.UnityInitializationInputProto.CustomResetParametersEntry)
    })
  ,
  'DESCRIPTOR' : _UNITYINITIALIZATIONINPUTPROTO,
  '__module__' : 'mlagents.envs.communicator_objects.unity_initialization_input_pb2'
  # @@protoc_insertion_point(class_scope:communicator_objects.UnityInitializationInputProto)
  })
_sym_db.RegisterMessage(UnityInitializationInputProto)
_sym_db.RegisterMessage(UnityInitializationInputProto.CustomResetParametersEntry)


DESCRIPTOR.has_options = True
DESCRIPTOR._options = _descriptor._ParseOptions(descriptor_pb2.FileOptions(), _b('\252\002\034MLAgents.CommunicatorObjects'))
# @@protoc_insertion_point(module_scope)
